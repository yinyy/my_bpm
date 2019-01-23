using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Course.Common.Bll;
using Course.Core.Bll;
using Course.Core.Model;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Course.ashx
{
    /// <summary>
    /// CourseBranchHandler 的摘要说明
    /// </summary>
    public class CourseBranchHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<CourseBranchModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<CourseBranchModel>>(json);
                rpm.CurrentContext = context;
            }

            User currentUser = UserBll.Instance.GetUser(SysVisitor.Instance.UserId);
            CourseBranchModel model;

            switch (rpm.Action)
            {
                case "add":
                    model = new CourseBranchModel();
                    model.InjectFrom(rpm.Entity);
                    context.Response.Write(CourseBranchBll.Instance.Insert(model));
                    break;
                case "edit":
                    model = new CourseBranchModel();
                    model.InjectFrom(rpm.Entity);
                    model.KeyId = rpm.KeyId;
                    context.Response.Write(CourseBranchBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(CourseBranchBll.Instance.Delete(rpm.KeyId));
                    break;
                default:
                    context.Response.Write(CourseBranchBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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