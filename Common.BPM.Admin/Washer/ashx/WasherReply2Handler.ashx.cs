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
    /// WasherReply2Handler 的摘要说明
    /// </summary>
    public class WasherReply2Handler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherReply2Model>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherReply2Model>>(json);
                rpm.CurrentContext = context;
            }

            WasherReply2Model model;
            string filter;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    model.DepartmentId = departmentId;

                    context.Response.Write(WasherReply2Bll.Instance.Add(model));
                    break;
                case "edit":
                    model = rpm.Entity;
                    model.DepartmentId = departmentId;
                    model.KeyId = rpm.KeyId;

                    context.Response.Write(WasherReply2Bll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherReply2Bll.Instance.Delete(rpm.KeyId));
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherReply2Bll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        context.Response.Write(WasherReply2Bll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    }
                    break;
            }

            //context.Response.Flush();
            //context.Response.End();
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