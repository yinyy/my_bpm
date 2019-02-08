using BPM.Common;
using BPM.Core;
using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
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
                case "notice":
                    //把任务放到消息队列
                    string templateId = CommonSettingBll.Instance.Get("InvigilateTemplateID").Value;
                    string reminder = CommonSettingBll.Instance.Get("InvigilateReminder").Value;
                    string url = "http://course.dyzyxyydwlwsys.cc/PublicPlatform/Exam/InvigilateCalendar.aspx";

                    List<string> names1 = new List<string>();
                    List<string> names2 = new List<string>();
                    List<string> names3 = new List<string>();
                    foreach (var d in ExamStaffInvigilateDetailViewBll.Instance
                        .GetList(new ExamExamModel() { KeyId = rpm.KeyId })
                        .OrderBy(m => m.Started)
                        .Select(m => new
                        {
                            Staff = new
                            {
                                Id = m.StaffId,
                                Name = m.Name,
                                OpenId = m.OpenId
                            },
                            Data = new
                            {
                                title = new TemplateDataItem(m.ExamTitle),
                                date = new TemplateDataItem(string.Format("{0:yyyy年MM月dd日}", m.Started)),
                                time = new TemplateDataItem(string.Format("{0:HH:mm} - {1:HH:mm}", m.Started, m.Ended)),
                                address = new TemplateDataItem(m.Address),
                                remark = new TemplateDataItem(reminder),
                            }
                        }))
                    {
                        if (d.Staff.OpenId.StartsWith("open_"))
                        {
                            //还没有绑定
                            names2.Add(d.Staff.Name);
                        }
                        else
                        {
                            SendTemplateMessageResult result = TemplateApi.SendTemplateMessage(Config.SenparcWeixinSetting.WeixinAppId,
                                d.Staff.OpenId,
                                templateId,
                                url,
                                d.Data);
                            if (result.errcode == ReturnCode.请求成功)
                            {
                                names1.Add(d.Staff.Name);
                            }
                            else
                            {
                                names3.Add(d.Staff.Name);
                            }
                        }
                    }

                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Sended = string.Join(",", names1),
                        NotBinded = string.Join(",", names2),
                        Errored = string.Join(",", names3)
                    }));
                    break;
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