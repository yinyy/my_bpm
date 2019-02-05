using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Business;
using Exam.Core.Model;
using Newtonsoft.Json;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.Exam.ashx
{
    /// <summary>
    /// ExamHandler 的摘要说明
    /// </summary>
    public class ExamHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];

            if (action == "section_list")
            {
                int examId = Convert.ToInt32(context.Request["id"]);
                CommonStaffModel staff = null;
                string oauthUrl = null;
                if (context.Session["openid"] != null)
                {
                    staff = CommonStaffBll.Instance.Get(context.Session["openid"].ToString());
                }
                else
                {
                    string state = "Exam-" + SystemTime.Now.Millisecond;//随机数，用于识别请求可靠性
                    context.Session.Add("State", state);//储存随机数到Session
                    string nextUrl = string.Format("http://course.dyzyxyydwlwsys.cc/PublicPlatform/Exam/ExamDetail.aspx?id={0}", examId);
                    string returnUrl = string.Format("http://course.dyzyxyydwlwsys.cc/PublicPlatform/ashx/OAuth2Handler.ashx?action=userinfo&nextUrl={0}", nextUrl.UrlEncode());
                    oauthUrl = OAuthApi.GetAuthorizeUrl(Config.SenparcWeixinSetting.WeixinAppId, returnUrl, state, OAuthScope.snsapi_userinfo);
                }

                ExamExamModel exam = ExamExamBll.Instance.Get(examId);
                ExamExamSectionViewModel[] datas = ExamExamSectionViewBll.Instance.GetList(exam);
                context.Response.Write(JsonConvert.SerializeObject(new
                {
                    Success = true,
                    Exam = new
                    {
                        ID = exam.KeyId,
                        Status = exam.Started.Date > DateTime.Now.Date ? 0 : exam.Ended.Date >= DateTime.Now.Date ? 1 : 2,
                        Freezed = exam.Freezed,
                        Booking = (exam.SecKillStarted <= DateTime.Now) && (exam.SecKillEnded >= DateTime.Now) ? 1 : 0
                    },
                    Sections = datas.Select(m => new
                    {
                        ID = m.ExamSectionId,
                        Title = m.Title,
                        Date = string.Format("{0:yyyy年MM月dd日}", m.Started),
                        Time = string.Format("{0:HH:mm} - {1:HH:mm}", m.Started, m.Ended),
                        ItemCount = m.ItemCount == null ? 0 : m.ItemCount.Value,
                        TeacherRequired = m.TeacherRequired == null ? 0 : m.TeacherRequired.Value,
                        Choosed = staff == null || string.IsNullOrWhiteSpace(m.TeacherNames) ? false : m.TeacherNames.Contains(string.Format("{0},{1}", staff.KeyId, staff.Name))
                    }).ToArray(),
                    OAuthUrl = oauthUrl
                }));
            }
            else if (action == "exam_list")
            {
                ExamExamModel[] datas = ExamExamBll.Instance.GetAll();
                var obj = new
                {
                    Success = true,
                    Data = datas.Select(d => new
                    {
                        ID = d.KeyId,
                        Title = d.Title,
                        Memo = d.Memo,
                        Time = d.Started == d.Ended ? string.Format("{0:yyyy年MM月dd日}", d.Started) :
                        d.Started.Month == d.Ended.Month ? string.Format("{0:yyyy年MM月dd日} - {1:dd日}", d.Started, d.Ended) :
                        d.Started.Year == d.Ended.Year ? string.Format("{0:yyyy年MM月dd日} - {1:MM月dd日}", d.Started, d.Ended) :
                        string.Format("{0:yyyy年MM月dd日} - {1:yyyy年MM月dd日}", d.Started, d.Ended),
                        Status = d.Started.Date > DateTime.Now.Date ? 0 : d.Ended.Date >= DateTime.Now.Date ? 1 : 2,
                        Freezed = d.Freezed,
                        Booking = (d.SecKillStarted <= DateTime.Now) && (d.SecKillEnded >= DateTime.Now) ? 1 : 0
                    }).ToArray()
                };
                context.Response.Write(JsonConvert.SerializeObject(obj));
            }
            else if (action == "select")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                if (staff == null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到教师信息。" }));
                }
                else
                {
                    int examSectionId = Convert.ToInt32(context.Request["examSectionId"]);
                    ExamExamSectionModel examSection = ExamExamSectionBll.Instance.Get(examSectionId);
                    if (examSection == null)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到场次信息。" }));
                    }
                    else
                    {
                        ExamExamModel exam = ExamExamBll.Instance.Get(examSection.ExamId);
                        if (exam == null)
                        {
                            context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到考试信息。" }));
                        }
                        else if (exam.SecKillStarted > DateTime.Now)
                        {
                            context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "还没有开启预约。" }));
                        }
                        else if (exam.SecKillEnded < DateTime.Now)
                        {
                            context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "预约已经结束。" }));
                        }
                        else
                        {
                            bool finished = false;
                            int res = -1;

                            Action<int> resultAction = (c) =>
                            {
                                finished = true;
                                res = c;
                            };
                            ExamSelectionQueue.Instance.AddTask(examSectionId, staff.KeyId, true, resultAction);
                            ExamSelectionQueue.Instance.Start();

                            while (!finished) ;

                            context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Code = res }));
                        }
                    }
                }

            }
            else if (action == "unselect")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                if (staff == null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到教师信息。" }));
                }
                else
                {
                    int examSectionId = Convert.ToInt32(context.Request["examSectionId"]);
                    ExamExamSectionModel examSection = ExamExamSectionBll.Instance.Get(examSectionId);
                    if (examSection == null)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到场次信息。" }));
                    }
                    else
                    {
                        bool finished = false;
                        int res = -1;

                        Action<int> resultAction = (c) =>
                        {
                            finished = true;
                            res = c;
                        };
                        ExamSelectionQueue.Instance.AddTask(examSectionId, staff.KeyId, false, resultAction);
                        ExamSelectionQueue.Instance.Start();

                        while (!finished) ;

                        context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Code = res }));
                    }
                }

            }else if (action == "calendar")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                if (staff == null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到教师信息。" }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Data = ExamStaffInvigilateDetailViewBll.Instance.GetList(staff).Where(m => m.Ended > DateTime.Now).OrderBy(m => m.Started).Select(
                            m => new
                            {
                                Address = m.Address,
                                Date = string.Format("{0:yyyy年MM月dd日}", m.Started),
                                Time = string.Format("{0:HH:mm} - {1:HH:mm}", m.Started, m.Ended),
                                ExamSectionItemStaffId = m.ExamSectionItemStaffId,
                                ExamTitle = m.ExamTitle,
                                ExamSectionTitle = m.ExamSectionTitle,
                                Confirmed = m.Confirmed,
                                CanConfirm = m.Started>=DateTime.Now.AddDays(1)
                            }).ToArray()
                    }));
                }
            }else if (action == "invigilate_list")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                if (staff == null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Message = "没有找到教师信息。" }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Data = ExamStaffInvigilateDetailViewBll.Instance.GetList(staff).Where(m => m.Ended <= DateTime.Now).OrderBy(m => m.Started).Select(
                            m => new
                            {
                                Address = m.Address,
                                Date = string.Format("{0:yyyy年MM月dd日}", m.Started),
                                Time = string.Format("{0:HH:mm} - {1:HH:mm}", m.Started, m.Ended),
                                ExamSectionItemStaffId = m.ExamSectionItemStaffId,
                                ExamTitle = m.ExamTitle,
                                ExamSectionTitle = m.ExamSectionTitle,
                                Confirmed = m.Confirmed
                            }).ToArray()
                    }));
                }
            }else if (action == "confirm")
            {
                int id = Convert.ToInt32(context.Request["id"]);
                ExamExamSectionItemStaffModel eesis = ExamExamSectionItemStaffBll.Instance.Get(id);
                eesis.Confirmed = 1;
                if (ExamExamSectionItemStaffBll.Instance.Update(eesis) > 0)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                }
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