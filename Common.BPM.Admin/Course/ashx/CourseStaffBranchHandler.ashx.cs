using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Course.Common.Bll;
using Course.Core.Bll;
using Course.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Course.ashx
{
    /// <summary>
    /// CourseStaffBranchHandler 的摘要说明
    /// </summary>
    public class CourseStaffBranchHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<CourseStaffBranchModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<CourseStaffBranchModel>>(json);
                rpm.CurrentContext = context;
            }

            CourseStaffBranchModel model;

            switch (rpm.Action)
            {
                case "audit":
                    CourseStaffBranchModel[] array = CourseStaffBranchBll.Instance.Get(CommonStaffBll.Instance.Get(rpm.Entity.StaffId));
                    foreach(CourseStaffBranchModel csbm in array)
                    {
                        if (csbm.BranchId == rpm.Entity.BranchId)
                        {
                            csbm.Accepted = 1;
                            csbm.AcceptTime = DateTime.Now;
                            CourseStaffBranchBll.Instance.Update(csbm);
                        }
                        else
                        {
                            csbm.AcceptTime = DateTime.Now;
                            csbm.Accepted = 0;
                            CourseStaffBranchBll.Instance.Update(csbm);
                        }
                    }

                    context.Response.Write(1);
                    break;
                default:
                    context.Response.Write(CourseStaffBranchBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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