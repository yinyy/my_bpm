using Course.Common.Bll;
using Course.Common.Model;
using Course.Extension.Model;
using Newtonsoft.Json;
using Quartz;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace BPM.Admin.Job
{
    public class TeacherClassSchedulerJobTask : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            int type = Convert.ToInt32(context.JobDetail.JobDataMap.Get("type"));
            string templateId = CommonSettingBll.Instance.Get("SchedulerTemplateID").Value;
            string reminder = CommonSettingBll.Instance.Get("SchedulerReminder").Value;

            string url = "";
            if (type == 1)
            {
                //url = string.Format("http://zljk.dyzyxyydwlwsys.cc/GetTeacherCourseSchedule.hxl?date={0}", string.Format("{0:yyyyMMdd}", DateTime.Today.AddDays(1)));
                url = string.Format("http://zljk.dyzyxyydwlwsys.cc/GetTeacherCourseSchedule.hxl?date=20181114");
            }
            else if (type == 2)
            {
                //url = string.Format("http://zljk.dyzyxyydwlwsys.cc/GetTeacherCourseSchedule.hxl?date={0}", string.Format("{0:yyyyMMdd}", DateTime.Today));
                url = string.Format("http://zljk.dyzyxyydwlwsys.cc/GetTeacherCourseSchedule.hxl?date=20181114");
            }

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                {
                    string data = reader.ReadToEnd();
                    var obj = new { Success = false, Data = new CourseSchedulerModel[] { } };
                    obj = JsonConvert.DeserializeAnonymousType(data, obj);

                    if (obj.Success == true)
                    {
                        foreach (var d in obj.Data)
                        {
                            CommonStaffModel staff = CommonStaffBll.Instance.GetBySerial(d.TeacherSerial);
                            if (staff == null)
                            {
                                continue;
                            }

                            //只向绑定的用户发送信息
                            if (staff.OpenId.StartsWith("open_"))
                            {
                                continue;
                            }

                            #region 只给我一个人做测试
                            //if (staff.Serial != "2003030")
                            //{
                            //    continue;
                            //}
                            #endregion

                            var templateData = new
                            {
                                course = new TemplateDataItem(d.CourseTitle),
                                vclass = new TemplateDataItem(d.ClassTitle),
                                section = new TemplateDataItem(d.Section),
                                address = new TemplateDataItem(d.RoomTitle),
                                remark = new TemplateDataItem(reminder),
                            };

                            SendTemplateMessageResult result = TemplateApi.SendTemplateMessage(Config.SenparcWeixinSetting.WeixinAppId,
                                staff.OpenId,
                                templateId,
                                "",
                                templateData);
                        }
                    }
                }
            }
        }
    }
}