using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Dal;
using BPM.Core.Model;
using BPM.Common;
using BPM.Common.ValidateCode;

namespace BPM.Admin.ashx
{
    /// <summary>
    /// LoginHandler 的摘要说明
    /// </summary>
    public class LoginHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            var userName = context.Request["username"];
            var password = context.Request["password"];
            var validateCode = context.Request["validateCode"];
            var saveCookieDays = PublicMethod.GetInt(context.Request["savedays"]);

            var msg = new { success = false, message = "亲,用户名不存在哦！仔细猜一哈。" };

            var useValidateCode = ConfigHelper.GetValue("showValidateCode");

            if( useValidateCode == "true" && !VcodePage.Validation(validateCode))
            {
                msg = new {success = false, message = "亲,验证码不正确。"};
            }
            else
            {
                User u = UserDal.Instance.GetUserBy(userName);
                if(u!=null)
                {
                    if(!u.IsDisabled)
                    {
                        bool flag = UserBll.Instance.UserLogin(userName, password, saveCookieDays);
                        if(flag)
                        {
                            msg = new {success = true, message = "ok"};
                        }
                        else
                        {
                            msg = new {success = false, message = "亲，用户名或密码不正确哦。"};
                        }
                    }
                    else
                    {
                        msg = new {success = false, message = "亲，您的帐号已被禁用，请联系管理员吧。"};
                    }
                }
            }
            context.Response.Write(JSONhelper.ToJson(msg));
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