using BPM.Common;
using BPM.Core;
using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Exam.ashx
{
    /// <summary>
    /// ExamExamSectionItemHandler 的摘要说明
    /// </summary>
    public class ExamExamSectionItemHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<ExamExamSectionItemModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<ExamExamSectionItemModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter = null;
            ExamExamSectionItemModel model;

            switch (rpm.Action)
            {
                case "add":
                    model = new ExamExamSectionItemModel();
                    model.InjectFrom(rpm.Entity);

                    context.Response.Write(ExamExamSectionItemBll.Instance.Insert(model));
                    break;
                case "edit":
                    model = new ExamExamSectionItemModel();
                    model.InjectFrom(rpm.Entity);
                    model.KeyId = rpm.KeyId;

                    context.Response.Write(ExamExamSectionItemBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(ExamExamSectionItemBll.Instance.Delete(rpm.KeyId));
                    break;
                case "import":
                    int count = 0;

                    string data = rpm.Entity.Memo;
                    string[] list = data.Split('\n');
                    foreach(string item in list)
                    {
                        if (string.IsNullOrEmpty(item)){
                            continue;
                        }

                        string[] datas = item.Trim().Split(',');

                        model = new ExamExamSectionItemModel();
                        model.ExamSectionId = rpm.Entity.ExamSectionId;
                        model.Address = datas[1];
                        model.Subject = datas[0];
                        model.StudentCount = Convert.ToInt32(datas[2]);
                        model.TeacherCount = Convert.ToInt32(datas[3]);
                        model.Memo = "";

                        ExamExamSectionItemBll.Instance.Insert(model);

                        count++;
                    }

                    context.Response.Write(count);
                    break;
                case "invigilator":
                    //全部监考员
                    CommonStaffModel[] all = CommonStaffBll.Instance.GetTeachers();
                    //本场考试已经安排的监考人员
                    ExamStaffInvigilateDetailViewModel[] arranged = ExamStaffInvigilateDetailViewBll.Instance.GetList(new ExamExamSectionModel() {KeyId = rpm.Entity.ExamSectionId });
                    //有意参加本场监考的人员
                    ExamExamSectionStaffModel[] wanted = ExamExamSectionStaffBll.Instance.GetList(new ExamExamSectionModel() { KeyId=rpm.Entity.ExamSectionId});
                    //本考场已经安排的监考员
                    ExamExamSectionItemStaffModel[] current = ExamExamSectionItemStaffBll.Instance.GetList(new ExamExamSectionItemModel() { KeyId = rpm.KeyId });

                    var q = all.Select(m => new
                    {
                        KeyId = m.KeyId,
                        Serial = m.Serial,
                        Name = m.Name,
                        Arranged = arranged.Where(a => a.StaffId == m.KeyId).Count() > 0,
                        Current = current.Where(c => c.ExamSectionItemId == rpm.KeyId && c.StaffId == m.KeyId).Count() > 0,
                        Wanted = wanted.Where(w => w.StaffId == m.KeyId).Count() > 0
                    });
                    
                    context.Response.Write(JsonConvert.SerializeObject(q.OrderBy(m=>m.Name).ToArray()));
                    break;
                case "manual":
                    int res = 0;

                    //先删除已经安排的监考员信息
                    res+=ExamExamSectionItemStaffBll.Instance.Delete(new ExamExamSectionItemModel() { KeyId = rpm.KeyId });
                    if (!string.IsNullOrWhiteSpace(rpm.Entity.Memo))
                    {
                        foreach(string id in rpm.Entity.Memo.Split(',')){
                            ExamExamSectionItemStaffModel eesism = new ExamExamSectionItemStaffModel();
                            eesism.ExamSectionItemId = rpm.KeyId;
                            eesism.StaffId = Convert.ToInt32(id);

                            res+=ExamExamSectionItemStaffBll.Instance.Insert(eesism);
                        }
                    }

                    context.Response.Write(res);
                    break;
                default:
                    if (string.IsNullOrEmpty(rpm.Filter))
                    {
                        filter = "{\"groupOp\":\"AND\",\"rules\":[{\"field\":\"ExamSectionId\",\"op\":\"eq\",\"data\":\"0\"}],\"groups\":[]}";
                    }
                    context.Response.Write(ExamExamSectionItemStaffBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, string.IsNullOrEmpty(filter) ? rpm.Filter : filter, rpm.Sort, rpm.Order));
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