using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherReplyHandler 的摘要说明
    /// </summary>
    public class WasherReplyHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherReplyModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherReplyModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherReplyModel reply;
            switch (rpm.Action)
            {
                case "update":
                    reply = WasherReplyBll.Instance.Get(departmentId, rpm.Entity.Kind);
                    if (reply == null)
                    {
                        reply = new WasherReplyModel();
                        reply.Body = rpm.Entity.Body;
                        reply.DepartmentId = departmentId;
                        reply.Kind = rpm.Entity.Kind;
                        context.Response.Write(WasherReplyBll.Instance.Add(reply));
                    }
                    else
                    {
                        reply.Body = rpm.Entity.Body;
                        context.Response.Write(WasherReplyBll.Instance.Update(reply));
                    }

                    break;
                default:
                    reply = WasherReplyBll.Instance.Get(departmentId, rpm.Entity.Kind);
                    context.Response.Write(JSONhelper.ToJson(reply));
                    break;
            }
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