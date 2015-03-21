using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using Sanitation.Model;
using Sanitation.Bll;

using Omu.ValueInjecter;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Common;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class SanitationDispatchHandler : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<SanitationDispatchModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<SanitationDispatchModel>>(json);
                rpm.CurrentContext = context;
            }

            SanitationDispatchModel d;
            switch (rpm.Action)
            {
                default:
                    context.Response.Write(SanitationDispatchBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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