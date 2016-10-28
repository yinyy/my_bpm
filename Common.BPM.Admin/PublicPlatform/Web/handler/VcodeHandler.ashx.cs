using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// VcodeHandler 的摘要说明
    /// </summary>
    public class VcodeHandler : IHttpHandler, IRequiresSessionState
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
                    int deptId= Convert.ToInt16( context.Session["deptId"].ToString());
                    Department dept = DepartmentBll.Instance.Get(deptId);
                    WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);

                    string cid = setting.Sms.Cid;
                    string uid = setting.Sms.Uid;
                    string pas = setting.Sms.Pas;
                    string url = setting.Sms.Url;
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