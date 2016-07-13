using BPM.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// VcodeHandler 的摘要说明
    /// </summary>
    public class VcodeHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string telphone = context.Request.Params["telphone"].Trim();

            if (telphone.Length != 11)
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "电话号码错误。" }));
            }
            else {
                WasherVcodeModel vcode = WasherVcodeBll.Instance.Create(telphone);
                if (vcode == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "验证码创建失败。" }));
                }
                else
                {
                    string cid = "HxaWuiiIhLLu";
                    string uid = "crmUAROrBnL7";
                    string pas = "5maafaja";
                    string url = "http://api.weimi.cc/2/sms/send.html";
                    byte[] byteArray = Encoding.UTF8.GetBytes("mob=" + telphone + "&cid=" + cid + "&uid=" + uid + "&pas=" + pas + "&p1=" + vcode.Vcode + "&type=json");

                    HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(url));
                    webRequest.Method = "POST";
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    webRequest.ContentLength = byteArray.Length;
                    Stream newStream = webRequest.GetRequestStream();
                    newStream.Write(byteArray, 0, byteArray.Length);
                    newStream.Close();

                    HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                    StreamReader php = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    string message = php.ReadToEnd();

                    var obj = new { code = 0, msg = "" };
                    obj = JsonConvert.DeserializeAnonymousType(message, obj);
                    context.Response.Write(JSONhelper.ToJson(new { Success = obj.code == 0, Message = obj.msg }));
                }
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
    }
}