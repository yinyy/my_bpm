using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.TenPayLibV3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

//http://www.2cto.com/weixin/201505/396497.html
//下载对账单：http://www.2cto.com/weixin/201505/397142.html

namespace BPM.Admin.PublicPlatform.Web
{
    /// <summary>
    /// PaymentCallback 的摘要说明
    /// </summary>
    public class PaymentCallback : IHttpHandler
    {

        //TODO:此处需要更多判断验证回调的真实性
        public void ProcessRequest(HttpContext context)
        {
            String notifyData = GetNotifyData(context.Request.InputStream);
            notifyData = notifyData.Substring(notifyData.IndexOf("<"));

            var res = XDocument.Parse(notifyData);
            Dictionary <string, string> dics = new Dictionary<string, string>();
            foreach (XElement xe in res.Element("xml").Elements())
            {
                dics.Add(xe.Name.ToString(), xe.Value);
            }
            
            //设置支付参数
            RequestHandler requestHandler = new RequestHandler(context);
            //通信成功
            if (dics["return_code"] == "SUCCESS" &&  dics["result_code"] == "SUCCESS")
            {
                if (ValidateSignature(DepartmentBll.Instance.GetMerchantKey(dics["mch_id"]), dics))
                {
                    WasherOrderModel order = WasherOrderBll.Instance.Get(dics["out_trade_no"]);
                    try
                    {
                        order.TransactionId = dics["transaction_id"];
                        order.Status = "已支付";

                        if (WasherOrderBll.Instance.Update(order) > 0)
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

            using(StreamWriter writer = new StreamWriter(context.Server.MapPath(string.Format("~/log/{0}-callback.txt",DateTime.Now))))
            {
                writer.Write(TenPayV3.Unifiedorder(data));
            }
        }

        private bool ValidateSignature(string key, Dictionary<string,string> dics)
        {
            if(string.IsNullOrEmpty(key)|| dics == null || dics.Count == 0)
            {
                return false;
            }

            string str = "";
            foreach(var d in dics.Where(d => d.Key != "sign" && !string.IsNullOrWhiteSpace(d.Value)).OrderBy(d => d.Key))
            {
                if (str != "")
                {
                    str += "&";
                }
                str += d.Key + "=" + d.Value;
            }

            str += "&key="+key;

            MD5 md5 = MD5.Create();
            var bs = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
            var sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2"));
            }
            string sign = sb.ToString().ToUpper();

            return sign==dics["sign"];
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private string GetNotifyData(Stream stream)
        {
            Int32 len = Convert.ToInt32(stream.Length);
            byte[] bs = new byte[len];
            stream.Read(bs, 0, len);
            return Encoding.UTF8.GetString(bs).Trim().Replace("\r\n", "");
        }
    }
}