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
                    {
                        int res = 0;
                        foreach (var staff in CommonStaffBll.Instance.GetTeachers())
                        {
                            model = ExamStaffInvigilateBll.Instance.Get(staff);
                            if (model == null)
                            {
                                model = new ExamStaffInvigilateModel();
                                model.StaffId = staff.KeyId;
                                model.Invigilated = 0;
                                model.Created = DateTime.Now;
                                model.Updated = DateTime.Now;
                                model.AutoArranged = 1;

                                res += ExamStaffInvigilateBll.Instance.Insert(model);
                            }
                            else
                            {
                                model.Invigilated = 0;
                                model.Updated = DateTime.Now;

                                res += ExamStaffInvigilateBll.Instance.Update(model);
                            }
                        }
                        context.Response.Write(res);
                        break;
                    }
                //case "reset":
                //    {
                //        CommonStaffModel staff = CommonStaffBll.Instance.Get(rpm.KeyId);
                //        model = ExamStaffInvigilateBll.Instance.Get(staff);
                //        if (model == null)
                //        {
                //            model = new ExamStaffInvigilateModel();
                //            model.StaffId = staff.KeyId;
                //            model.Invigilated = rpm.Entity.Invigilated;
                //            model.Created = DateTime.Now;
                //            model.Updated = DateTime.Now;

                //            context.Response.Write(ExamStaffInvigilateBll.Instance.Insert(model));
                //        }
                //        else
                //        {
                //            model.Invigilated = rpm.Entity.Invigilated;
                //            model.Updated = DateTime.Now;

                //            context.Response.Write(ExamStaffInvigilateBll.Instance.Update(model));
                //        }
                //        break;
                //    }
                case "add":
                    {
                        var obj = new { Serial = "", Name = "", Gender = "", Invigilated = 0, AutoArranged = 0 };
                        obj = JsonConvert.DeserializeAnonymousType(rpm.JsonEntity, obj);

                        CommonStaffModel staff = CommonStaffBll.Instance.Get(obj.Serial, obj.Name, "teacher");
                        if (staff == null)
                        {
                            staff = new CommonStaffModel();
                            staff.InjectFrom(obj);
                            staff.OpenId = string.Format("open_{0}", Convert.ToString(DateTime.Now.Ticks, 16));
                            staff.Type = "teacher";

                            staff.KeyId = CommonStaffBll.Instance.Insert(staff);
                        }

                        ExamStaffInvigilateModel esi = new ExamStaffInvigilateModel();
                        esi.InjectFrom(rpm.Entity);
                        esi.StaffId = staff.KeyId;
                        esi.Created = DateTime.Now;
                        esi.Updated = DateTime.Now;

                        context.Response.Write(ExamStaffInvigilateBll.Instance.Insert(esi));
                        break;
                    }
                case "edit":
                    {
                        ExamStaffInvigilateModel esi = ExamStaffInvigilateBll.Instance.Get(rpm.KeyId);
                        esi.Updated = DateTime.Now;
                        esi.AutoArranged = rpm.Entity.AutoArranged;
                        esi.Invigilated = rpm.Entity.Invigilated;

                        context.Response.Write(ExamStaffInvigilateBll.Instance.Update(esi));
                        break;
                    }
                case "import":
                    {
                        var obj = new { Memo = "" };
                        obj = JsonConvert.DeserializeAnonymousType(rpm.JsonEntity, obj);
                        if (!string.IsNullOrWhiteSpace(obj.Memo))
                        {
                            string[] datas = obj.Memo.Split('\n');

                            Random random = new Random();

                            foreach(string d in datas)
                            {
                                if (string.IsNullOrWhiteSpace(d))
                                {
                                    continue;
                                }

                                string[] values = d.Trim().Split(',');
                                if (string.IsNullOrWhiteSpace(values[0]))
                                {
                                    continue;
                                }

                                CommonStaffModel staff = CommonStaffBll.Instance.GetBySerial(values[0]);
                                if (staff != null)
                                {
                                    continue;
                                }

                                staff = new CommonStaffModel();
                                staff.OpenId = string.Format("open_{0}_{1}", Convert.ToString(DateTime.Now.Ticks, 16), random.Next(1000));
                                staff.Serial = values[0].Trim();
                                staff.Name = values[1].Trim();
                                staff.Gender = values[2].Trim();
                                staff.Type = "teacher";

                                staff.KeyId = CommonStaffBll.Instance.Insert(staff);

                                ExamStaffInvigilateModel esi = new ExamStaffInvigilateModel();
                                esi.StaffId = staff.KeyId;
                                esi.Created = DateTime.Now;
                                esi.Updated = DateTime.Now;
                                esi.Invigilated = Convert.ToInt32(values[3]);
                                esi.AutoArranged = Convert.ToInt32(values[4]);

                                ExamStaffInvigilateBll.Instance.Insert(esi);
                            }
                        }

                        context.Response.Write(1);
                        break;
                    }
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