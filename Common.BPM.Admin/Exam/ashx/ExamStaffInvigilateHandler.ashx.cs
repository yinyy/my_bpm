using BPM.Common;
using BPM.Core;
using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Exam.ashx
{
    /// <summary>
    /// ExamStaffInvigilateHandler 的摘要说明
    /// </summary>
    public class ExamStaffInvigilateHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ExamStaffInvigilateModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ExamStaffInvigilateModel>>(json);
                rpm.CurrentContext = context;
            }

            ExamStaffInvigilateModel model = null;

            switch (rpm.Action)
            {
                case "init":
                    int res = 0;
                    foreach(var m in CommonStaffBll.Instance.GetTeachers())
                    {
                        model = ExamStaffInvigilateBll.Instance.Get(m);
                        if (model == null)
                        {
                            model = new ExamStaffInvigilateModel();
                            model.StaffId = m.KeyId;
                            model.Invigilated = 0;
                            model.Created = DateTime.Now;
                            model.Updated = DateTime.Now;

                            res += ExamStaffInvigilateBll.Instance.Insert(model);
                        }
                        else
                        {
                            model.Invigilated = 0;
                            model.Created = DateTime.Now;
                            model.Updated = DateTime.Now;

                            res += ExamStaffInvigilateBll.Instance.Update(model);
                        }
                    }
                    context.Response.Write(res);
                    break;
                case "reset":
                    CommonStaffModel staff = CommonStaffBll.Instance.Get(rpm.KeyId);
                    model = ExamStaffInvigilateBll.Instance.Get(staff);
                    if (model == null)
                    {
                        model = new ExamStaffInvigilateModel();
                        model.StaffId = staff.KeyId;
                        model.Invigilated = rpm.Entity.Invigilated;
                        model.Created = DateTime.Now;
                        model.Updated = DateTime.Now;

                        context.Response.Write(ExamStaffInvigilateBll.Instance.Insert(model));
                    }
                    else
                    {
                        model.Invigilated = rpm.Entity.Invigilated;
                        model.Updated = DateTime.Now;

                        context.Response.Write(ExamStaffInvigilateBll.Instance.Update(model));
                    }
                    break;
                default:
                    context.Response.Write(ExamStaffInvigilateBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    break;
            }

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