using BPM.Common;
using BPM.Core;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Exam.ashx
{
    /// <summary>
    /// ExamExamHandler 的摘要说明
    /// </summary>
    public class ExamExamHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ExamExamModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ExamExamModel>>(json);
                rpm.CurrentContext = context;
            }
            ExamExamModel model;

            switch (rpm.Action)
            {
                case "add":
                    model = new ExamExamModel();
                    model.InjectFrom(rpm.Entity);
                    context.Response.Write(ExamExamBll.Instance.Insert(model));
                    break;
                case "edit":
                    model = new ExamExamModel();
                    model.InjectFrom(rpm.Entity);
                    model.KeyId = rpm.KeyId;

                    context.Response.Write(ExamExamBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(ExamExamBll.Instance.Delete(rpm.KeyId));
                    break;
                case "freeze":
                    int res = 0;
                    ExamStaffInvigilateDetailViewModel[] datas = ExamStaffInvigilateDetailViewBll.Instance.GetList(new ExamExamModel() { KeyId = rpm.KeyId});
                    foreach(var d in datas)
                    {
                        ExamStaffInvigilateModel esim = ExamStaffInvigilateBll.Instance.Get(new CommonStaffModel() { KeyId = d.StaffId});
                        if (d == null)
                        {
                            esim = new ExamStaffInvigilateModel();
                            esim.Created = DateTime.Now;
                            esim.Updated = DateTime.Now;
                            esim.Invigilated = 1;
                            esim.StaffId = d.StaffId;

                            res += ExamStaffInvigilateBll.Instance.Insert(esim);
                        }
                        else
                        {
                            esim.Invigilated += 1;

                            res = ExamStaffInvigilateBll.Instance.Update(esim);
                        }
                    }

                    ExamExamModel eem = ExamExamBll.Instance.Get(rpm.KeyId);
                    eem.Freezed = 1;
                    res += ExamExamBll.Instance.Update(eem);

                    context.Response.Write(res);
                    break;
                default:
                    context.Response.Write(ExamExamBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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