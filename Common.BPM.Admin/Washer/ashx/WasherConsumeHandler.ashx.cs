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
using Omu.ValueInjecter;
using BPM.Core.Dal;

namespace BPM.Admin.Washer.ashx
{
    //设备当前的状态，0表示状态位置，1表示当前正在工作，2表示空闲

    /// <summary>
    /// WasherDeviceHandler 的摘要说明
    /// </summary>
    public class WasherConsumeHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherConsumeModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherConsumeModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter;

            switch (rpm.Action)
            {
                case "export":
                    if (user.IsAdmin)
                    {
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherConsumeBll.Instance.Export(rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherConsumeBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                    }
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherConsumeBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        context.Response.Write(WasherConsumeBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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