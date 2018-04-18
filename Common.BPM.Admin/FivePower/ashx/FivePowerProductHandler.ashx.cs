using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using BPM.FivePower.Bll;
using BPM.FivePower.Model;
using System;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.FivePower.ashx
{
    /// <summary>
    /// FivePowerProductHandler 的摘要说明
    /// </summary>
    public class FivePowerProductHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<FivePowerProductModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<FivePowerProductModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(FivePowerProductBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        string filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        context.Response.Write(FivePowerProductBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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