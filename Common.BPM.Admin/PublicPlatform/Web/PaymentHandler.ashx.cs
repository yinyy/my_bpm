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
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Washer.Bll;
using Washer.Model;


//http://www.2cto.com/weixin/201505/396497.html


namespace BPM.Admin.PublicPlatform.Web
{
    /// <summary>
    /// PaymentHandler 的摘要说明
    /// </summary>
    public class PaymentHandler : IHttpHandler
    {
        private TenPayV3Info tenPayV3Info;

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];
            if ("create_pay_params" == action)
            {
                string wxid = context.Request.Params["wxid"];
                WasherWeChatConsumeModel wxconsume;
                Department dept;
                if (string.IsNullOrWhiteSpace(wxid) ||
                    (wxconsume = WasherWeChatConsumeBll.Instance.Get(Convert.ToInt32(wxid))) == null ||
                    (dept = DepartmentBll.Instance.Get(wxconsume.DepartmentId)) == null)
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
                    "http://xc.senlanjidian.com/PublicPlatform/Web/PaymentCallback.ashx");

                var ps = new
                {
                    wxid = Convert.ToInt32(context.Request.Params["wxid"]),
                    body = context.Request.Params["body"],
                    pay = Convert.ToInt32(context.Request.Params["pay"]),
                    attach = context.Request.Params["attach"]
                };

                string orderNo = CreateLocalOrder(ps.wxid, ps.body, ps.pay, ps.attach);
                if (orderNo != "error")
                {
                    string unifiedorder = GetUnifiedorder(ps.attach,
                        ps.body,
                    WasherWeChatConsumeBll.Instance.Get(ps.wxid).OpenId,
                    Convert.ToString(ps.pay), orderNo);
                    Match match = Regex.Match(unifiedorder, @"<prepay_id><\!\[CDATA\[(wx\S+)\]\]></prepay_id>");
                    if (match.Groups.Count > 0)
                    {
                        string prepayId = match.Groups[1].Value;
                        JObject jobj = GetPrepayInfo(prepayId);
                        jobj.Add("Seral", orderNo);

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
            }
            else if("validate_payment"==action)
            {
                string serial = context.Request.Params["serial"];

                WasherOrderModel order = WasherOrderBll.Instance.Get(serial);
                if (order.Status == "已支付")
                {
                    WasherDeviceModel device = WasherDeviceBll.Instance.GetByBoardNumber(order.Memo);
                    WasherConsumeModel consume = WasherConsumeBll.Instance.Get(device.DepartmentId, order.WeChatConsumeId);
                    
                    //将支付信息写入设备日志
                    WasherDeviceLogModel log = new WasherDeviceLogModel();
                    log.CardId = 0;
                    log.ConsumeId = consume.KeyId;
                    log.DeviceId = device.KeyId;
                    log.Kind = "微信支付";
                    log.Memo = "";
                    log.PayCoins = 0;
                    log.RemainCoins = order.Money;
                    log.Started = DateTime.Now;

                    if (WasherDeviceLogBll.Instance.Add(log) > 0)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true, BalanceId = log.KeyId, Money = order.Money }));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson( new { Success = false, Message="写入设备日志错误。" }));
                    }
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "订单未支付。" }));
                }
            }
        }

        private string CreateLocalOrder(int wxid, string kind, int pay, string memo)
        {
            WasherOrderModel order = new WasherOrderModel();
            order.WeChatConsumeId = wxid;
            order.Kind = kind;
            order.Memo = memo;
            order.Money = pay;
            order.Status = "未支付";
            order.Time = DateTime.Now;
            order.TransactionId = "";
            order.Serial = string.Format("{0:yyyyMMdd}{1:000000}{2:00000}", DateTime.Now, 
                WasherWeChatConsumeBll.Instance.Get(wxid).DepartmentId, (DateTime.Now-DateTime.Now.Date).TotalSeconds);

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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}