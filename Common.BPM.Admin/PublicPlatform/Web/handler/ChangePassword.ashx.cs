using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// ChangePassword 的摘要说明
    /// </summary>
    public class ChangePassword : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];
            string appid = context.Session["appid"].ToString();
            string openid = context.Session["openid"].ToString();

            Department dept = DepartmentBll.Instance.GetByAppid(appid);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

            if (action == "change")
            {
                String password = context.Request.Params["password"];

                consume.Password = password;

                if (WasherConsumeBll.Instance.Update(consume) > 0)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "修改密码时发生错误，请稍后重试。" }));
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