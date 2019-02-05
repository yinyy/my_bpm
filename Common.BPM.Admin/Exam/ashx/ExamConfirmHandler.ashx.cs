using BPM.Common;
using BPM.Core;
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
    /// ExamConfirmHandler 的摘要说明
    /// </summary>
    public class ExamConfirmHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ExamExamSectionItemStaffModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ExamExamSectionItemStaffModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                default:
                    var q = ExamStaffInvigilateDetailViewBll.Instance.GetList().Select(m => new
                    {
                        ExamId = m.ExamId,
                        ExamTitle = m.ExamTitle,
                        //ExamSectionItemStaffId = m.ExamSectionItemStaffId,
                        //StaffId = m.StaffId,
                        StaffName = m.Name,
                        Time = string.Format("{0:yyyy年MM月dd日 HH:mm} - {1:HH:mm}", m.Started, m.Ended),
                        Address = m.Address,
                        Confirmed = m.Confirmed
                    }).GroupBy(m => new { ExamId = m.ExamId, ExamTitle = m.ExamTitle })
                    .OrderByDescending(m => m.Key.ExamId)
                    .Select(g => new
                    {
                        ExamId = g.Key.ExamId,
                        ExamTitle = g.Key.ExamTitle,
                        Data = string.Join("；", g.Where(m => m.Confirmed == 0).OrderBy(m=>m.StaffName).ThenBy(m=>m.Time).Select(m => string.Format("{0}，{1}，{2}", m.StaffName, m.Time, m.Address)))
                    });

                    context.Response.Write(JsonConvert.SerializeObject(new { total = q.Count(), rows = q.ToArray() }));
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