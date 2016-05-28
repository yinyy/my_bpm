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
    /// WasherCardHandler 的摘要说明
    /// </summary>
    public class WasherDeviceLogHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCardModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCardModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter;
            switch (rpm.Action)
            {
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}",departmentId, rpm.Filter);
                        context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    }
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