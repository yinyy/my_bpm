using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// UnifyPay 的摘要说明
    /// </summary>
    public class UnifyPay : IHttpHandler, IRequiresSessionState
    {
        private TenPayV3Info tenPayV3Info;

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];
            if (string.IsNullOrWhiteSpace(action))
            {
                action = "";
            }

            switch (action)
            {
                case "prepay":
                    string appid = context.Session["appid"].ToString();
                    string openid = context.Session["openid"].ToString();

                    Department dept = DepartmentBll.Instance.GetByAppid(appid);
                    WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
                    if (wxconsume == null ||dept == null)
                    {
                        context.Response.Write("ERROR");
                        context.Response.End();

                        return;
                    }

                    tenPayV3Info = new TenPayV3Info(
                        dept.Appid,
                        dept.Secret,
                        dept.MerchantId,
                        dept.MerchantKey,
                        "http://xc.senlanjidian.com/PublicPlatform/Web/handler/UnifyPay.ashx");

                    var ps = new
                    {
                        Body = context.Request.Params["body"],
                        Pay = Convert.ToInt32(context.Request.Params["pay"]),
                        Attach = context.Request.Params["attach"]
                    };

                    string orderSerial = CreateLocalOrder(dept.KeyId, openid, ps.Body, ps.Pay, ps.Attach);
                    if (orderSerial != "error")
                    {
                        string unifiedorder = GetUnifiedorder(ps.Attach, ps.Body, openid, Convert.ToString(ps.Pay), orderSerial);
                        Match match = Regex.Match(unifiedorder, @"<prepay_id><\!\[CDATA\[(wx\S+)\]\]></prepay_id>");
                        if (match.Groups.Count > 0)
                        {
                            string prepayId = match.Groups[1].Value;
                            JObject jobj = GetPrepayInfo(prepayId);
                            jobj.Add("Serial", orderSerial);

                            context.Response.Write(JSONhelper.ToJson(new { Success = true, PrepayInfo = jobj.ToString() }));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = false }));
                        }
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false }));
                    }

                    break;
                case "validate":
                    string serial = context.Request.Params["serial"];
                    int checkCount = 5;
                    WasherOrderModel order = null;

                    do
                    {
                        order = WasherOrderBll.Instance.Get(serial);
                        if (order != null && order.Status == "已支付")
                        {
                            break;
                        }

                        Thread.Sleep(500);
                    } while (checkCount-- > 0);

                    if (order != null && order.Status == "已支付")
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "订单未支付。" }));
                    }
                    break;
                default:
                    String notifyData = GetNotifyData(context.Request.InputStream);
                    notifyData = notifyData.Substring(notifyData.IndexOf("<"));
                    
                    var res = XDocument.Parse(notifyData);
                    Dictionary<string, string> dics = new Dictionary<string, string>();
                    foreach (XElement xe in res.Element("xml").Elements())
                    {
                        dics.Add(xe.Name.ToString(), xe.Value);
                    }

                    //设置支付参数
                    RequestHandler requestHandler = new RequestHandler(context);
                    //通信成功
                    if (dics["return_code"] == "SUCCESS" && dics["result_code"] == "SUCCESS")
                    {
                        if (ValidateSignature(DepartmentBll.Instance.GetMerchantKey(dics["mch_id"]), dics))
                        {
                            WasherOrderModel om = WasherOrderBll.Instance.Get(dics["out_trade_no"]);
                            try
                            {
                                om.TransactionId = dics["transaction_id"];
                                om.Status = "已支付";

                                if (WasherOrderBll.Instance.Update(om) > 0)
                                {
                                    requestHandler.SetParameter("return_code", "SUCCESS");
                                    requestHandler.SetParameter("return_msg", "OK");
                                }
                            }
                            catch
                            {
                                requestHandler.SetParameter("return_code", "FAIL");
                                requestHandler.SetParameter("return_msg", "交易失败");
                            }
                        }
                        else
                        {
                            requestHandler.SetParameter("return_code", "FAIL");
                            requestHandler.SetParameter("return_msg", "签名失败");
                        }
                    }
                    else
                    {
                        requestHandler.SetParameter("return_code", "FAIL");
                        requestHandler.SetParameter("return_msg", "交易失败");
                    }

                    string data = requestHandler.ParseXML();
                    using (StreamWriter writer = new StreamWriter(context.Server.MapPath(string.Format("~/log/{0}-callback.txt", DateTime.Now))))
                    {
                        writer.Write(TenPayV3.Unifiedorder(data));
                    }

                    break;
            }

            context.Response.Flush();
            context.Response.End();
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string CreateLocalOrder(int deptId, string openid, string kind, int pay, string memo)
        {
            WasherOrderModel order = new WasherOrderModel();
            order.DepartmentId = deptId;
            order.OpenId = openid;
            order.Kind = kind;
            order.Memo = memo;
            order.Money = pay;
            order.Status = "未支付";
            order.Time = DateTime.Now;
            order.TransactionId = "";
            order.Serial = string.Format("{0:yyyyMMdd}{1:0000}{2:00000}", DateTime.Now, deptId, (DateTime.Now - DateTime.Now.Date).TotalSeconds);

            if (WasherOrderBll.Instance.Add(order) > 0)
            {
                return order.Serial;
            }
            else
            {
                return "error";
            }
        }

        private string GetUnifiedorder(string attach, string body, string openid, string price, string orderNum = "1833431773763549")
        {
            RequestHandler requestHandler = new RequestHandler(HttpContext.Current);

            //微信分配的公众账号ID（企业号corpid即为此appId）
            requestHandler.SetParameter("appid", tenPayV3Info.AppId);

            //附加数据，在查询API和支付通知中原样返回，该字段主要用于商户携带订单的自定义数据
            requestHandler.SetParameter("attach", attach);

            //商品或支付单简要描述
            requestHandler.SetParameter("body", body);

            //微信支付分配的商户号
            requestHandler.SetParameter("mch_id", tenPayV3Info.MchId);

            //随机字符串，不长于32位。
            requestHandler.SetParameter("nonce_str", TenPayV3Util.GetNoncestr());

            //接收微信支付异步通知回调地址，通知url必须为直接可访问的url，不能携带参数。
            requestHandler.SetParameter("notify_url", tenPayV3Info.TenPayV3Notify);

            //trade_type=JSAPI，此参数必传，用户在商户公众号appid下的唯一标识。
            requestHandler.SetParameter("openid", openid);

            //商户系统内部的订单号,32个字符内、可包含字母，自己生成
            requestHandler.SetParameter("out_trade_no", orderNum);

            //APP和网页支付提交用户端ip，Native支付填调用微信支付API的机器IP。
            requestHandler.SetParameter("spbill_create_ip", "127.0.0.1");

            //订单总金额，单位为分，做过银联支付的朋友应该知道，代表金额为12位，末位分分
            requestHandler.SetParameter("total_fee", price);

            //取值如下：JSAPI，NATIVE，APP，我们这里使用JSAPI
            requestHandler.SetParameter("trade_type", "JSAPI");

            //设置KEY
            //requestHandler.SetKey(tenPayV3Info.Key);

            requestHandler.SetParameter("sign", requestHandler.CreateMd5Sign("key", tenPayV3Info.Key));
            //requestHandler.GetRequestURL();
            //requestHandler.CreateSHA1Sign();

            string data = requestHandler.ParseXML();
            requestHandler.GetDebugInfo();

            //获取并返回预支付XML信息
            return TenPayV3.Unifiedorder(data);
        }

        private JObject GetPrepayInfo(string prepayId)
        {
            //检查是否已经注册jssdk
            if (!JsApiTicketContainer.CheckRegistered(tenPayV3Info.AppId))
            {
                JsApiTicketContainer.Register(tenPayV3Info.AppId, tenPayV3Info.AppSecret);
            }

            JsApiTicketResult jsApiTicket = JsApiTicketContainer.GetJsApiTicketResult(tenPayV3Info.AppId);
            JSSDKHelper jssdkHelper = new JSSDKHelper();

            JObject jobj = new JObject();
            jobj.Add("AppID", tenPayV3Info.AppId);
            jobj.Add("Ticket", jsApiTicket.ticket);
            jobj.Add("Noncestr", JSSDKHelper.GetNoncestr());
            jobj.Add("Timestamp", JSSDKHelper.GetTimestamp());
            jobj.Add("Package", "prepay_id=" + prepayId);

            RequestHandler requestHandler = new RequestHandler(HttpContext.Current);

            requestHandler.SetParameter("appId", jobj["AppID"].ToString());
            requestHandler.SetParameter("timeStamp", jobj["Timestamp"].ToString());
            requestHandler.SetParameter("nonceStr", jobj["Noncestr"].ToString());
            requestHandler.SetParameter("package", jobj["Package"].ToString());
            requestHandler.SetParameter("signType", "MD5");

            //requestHandler.SetKey(tenPayV3Info.Key);
            requestHandler.SetParameter("sign", requestHandler.CreateMd5Sign("key", tenPayV3Info.Key));
            //requestHandler.GetRequestURL();
            //requestHandler.CreateSHA1Sign();

            jobj["PaySign"] = (requestHandler.GetAllParameters()["sign"]).ToString();

            return jobj;
        }

        private string GetNotifyData(Stream stream)
        {
            Int32 len = Convert.ToInt32(stream.Length);
            byte[] bs = new byte[len];
            stream.Read(bs, 0, len);
            return Encoding.UTF8.GetString(bs).Trim().Replace("\r\n", "");
        }

        private bool ValidateSignature(string key, Dictionary<string, string> dics)
        {
            if (string.IsNullOrEmpty(key) || dics == null || dics.Count == 0)
            {
                return false;
            }

            string str = "";
            foreach (var d in dics.Where(d => d.Key != "sign" && !string.IsNullOrWhiteSpace(d.Value)).OrderBy(d => d.Key))
            {
                if (str != "")
                {
                    str += "&";
                }
                str += d.Key + "=" + d.Value;
            }

            str += "&key=" + key;

            MD5 md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            string sign = sb.ToString().ToUpper();

            return sign == dics["sign"];
        }
    }
}