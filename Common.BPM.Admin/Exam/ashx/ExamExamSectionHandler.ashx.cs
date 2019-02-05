using BPM.Common;
using BPM.Core;
using Course.Common.Bll;
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
    /// ExamExamSectionHandler 的摘要说明
    /// </summary>
    public class ExamExamSectionHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ExamExamSectionModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ExamExamSectionModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter = null;
            ExamExamSectionModel model;

            switch (rpm.Action)
            {
                case "add":
                    model = new ExamExamSectionModel();
                    model.InjectFrom(rpm.Entity);

                    context.Response.Write(ExamExamSectionBll.Instance.Insert(model));
                    break;
                case "edit":
                    model = new ExamExamSectionModel();
                    model.InjectFrom(rpm.Entity);
                    model.KeyId = rpm.KeyId;

                    context.Response.Write(ExamExamSectionBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(ExamExamSectionBll.Instance.Delete(rpm.KeyId));
                    break;
                case "auto":
                    {
                        ExamExamSectionModel ees = new ExamExamSectionModel();
                        ees.KeyId = rpm.KeyId;

                        //想要监考id的集合
                        var wanted = ExamExamSectionStaffBll.Instance.GetList(ees).Select(m => m.StaffId);
                        //本场次已经排过的id集合
                        var arranged = ExamStaffInvigilateDetailViewBll.Instance.GetList(ees).Select(m => m.StaffId);
                        //本场次还可以排的id集合
                        wanted = wanted.Except(arranged);

                        int res = 0;
                        while (wanted.Count() > 0)
                        {
                            int required = 0;
                            foreach (var eesi in ExamExamSectionItemBll.Instance.GetList(ees))
                            {
                                //拿出一个考场，看看这个考场已经安排的监考员和需求的监考员之间的关系
                                int count = ExamExamSectionItemStaffBll.Instance.GetList(eesi).Count();
                                if (count < eesi.TeacherCount)
                                {
                                    ExamExamSectionItemStaffModel eesis = new ExamExamSectionItemStaffModel();
                                    eesis.ExamSectionItemId = eesi.KeyId;
                                    eesis.StaffId = wanted.First();
                                    res += ExamExamSectionItemStaffBll.Instance.Insert(eesis);

                                    wanted = wanted.Skip(1);

                                    required += eesi.TeacherCount - count + 1;
                                }

                                if (wanted.Count() == 0)
                                {
                                    break;
                                }
                            }

                            if (required == 0)
                            {
                                break;
                            }
                        }

                        context.Response.Write(res);
                    }
                    break;
                case "fill":
                    {
                        ExamExamSectionModel ees = ExamExamSectionBll.Instance.Get(rpm.KeyId);
                        ExamExamModel ee = ExamExamBll.Instance.Get(ees.ExamId);

                        //监考员id的集合
                        var wanted = CommonStaffBll.Instance.GetTeachers().Select(m=>m.KeyId);
                        //本场次已经排过的id集合
                        var arranged = ExamStaffInvigilateDetailViewBll.Instance.GetList(ees).Select(m => m.StaffId);
                        //本场次还可以排的id集合
                        wanted = wanted.Except(arranged);
                        //监考员之前的监考次数+本次考试的监考次数之和
                        var wanted2 = wanted.Select(m => new
                        {
                            StaffId = m,
                            Invigilated = ExamStaffInvigilateBll.Instance.Get(new CommonStaffModel() { KeyId = m }).Invigilated +
                            ExamStaffInvigilateDetailViewBll.Instance.GetList(ee).GroupBy(g => g.StaffId).Count()
                        }).OrderBy(m => m.Invigilated).AsEnumerable();
                        
                        int res = 0;
                        while (wanted2.Count() > 0)
                        {
                            int required = 0;
                            foreach (var eesi in ExamExamSectionItemBll.Instance.GetList(ees))
                            {
                                //拿出一个考场，看看这个考场已经安排的监考员和需求的监考员之间的关系
                                int count = ExamExamSectionItemStaffBll.Instance.GetList(eesi).Count();
                                if (count < eesi.TeacherCount)
                                {
                                    ExamExamSectionItemStaffModel eesis = new ExamExamSectionItemStaffModel();
                                    eesis.ExamSectionItemId = eesi.KeyId;
                                    eesis.StaffId = wanted2.First().StaffId;
                                    res += ExamExamSectionItemStaffBll.Instance.Insert(eesis);

                                    wanted2 = wanted2.Skip(1);

                                    required += eesi.TeacherCount - count + 1;
                                }

                                if (wanted2.Count() == 0)
                                {
                                    break;
                                }
                            }

                            if (required == 0)
                            {
                                break;
                            }
                        }

                        context.Response.Write(res);
                    }
                    break;
                default:
                    if (string.IsNullOrEmpty(rpm.Filter))
                    {
                        filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamId\",\"op\":\"eq\",\"data\":\"0\"}],\"groups\":[]}";
                    }
                    context.Response.Write(ExamExamSectionBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, string.IsNullOrEmpty(filter) ? rpm.Filter : filter, rpm.Sort, rpm.Order));
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