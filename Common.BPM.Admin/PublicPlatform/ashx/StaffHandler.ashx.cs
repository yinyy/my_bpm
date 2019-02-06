using Course.Common.Bll;
using Course.Common.Model;
using Exam.Core.Bll;
using Exam.Core.Model;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.UserTag;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.ashx
{
    /// <summary>
    /// StaffHandler 的摘要说明
    /// </summary>
    public class StaffHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];

            if (action == "check")
            {
                string serial = context.Request["serial"];
                string type = context.Request["type"];
                string password = context.Request["password"];

                #region 从教学质量监控系统获取用户信息
                string url = string.Format("http://zljk.dyzyxyydwlwsys.cc/GetStaffInfo.hxl?serial={0}&type={1}&password={2}", serial, type, password);

                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Method = "GET";

                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        string data = reader.ReadToEnd();
                        var obj = new { Success = false, Data = new { Serial = "", Name = "", Type = "", Gender = "" }, Message = "" };
                        obj = JsonConvert.DeserializeAnonymousType(data, obj);
                        context.Response.Write(JsonConvert.SerializeObject(obj));
                    }
                }
                #endregion
            }
            else if (action == "get")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel staff = CommonStaffBll.Instance.Get(openid);
                if (staff == null || staff.Serial == null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Data = staff }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = staff }));
                }
            }
            else if (action == "bind")
            {
                string serial = context.Request["serial"];
                string name = context.Request["name"];
                string type = context.Request["type"];
                string openid = context.Session["openid"].ToString();
                string gender = context.Request["gender"];

                CommonStaffModel staff = CommonStaffBll.Instance.Get(serial, name, type);
                if (staff != null && !staff.OpenId.StartsWith("open_"))
                {
                    //说明已经被绑定过了
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false, Data = serial, Code = -1 }));
                    return;
                }
                else if (staff != null)
                {
                    //删除只有openid信息的那条记录
                    CommonStaffModel delStaff = CommonStaffBll.Instance.Get(openid);
                    CommonStaffBll.Instance.Delete(delStaff.KeyId);

                    staff.OpenId = openid;
                }
                else
                {
                    staff = CommonStaffBll.Instance.Get(openid);
                }

                staff.Serial = serial;
                staff.Name = name;
                staff.Gender = gender;
                staff.Type = type;

                CommonStaffBll.Instance.Update(staff);

                //给当前用户打上相应的标签
                List<string> openids = new List<string>();
                openids.Add(openid);

                if (staff.Type == "student")
                {
                    UserTagApi.BatchTagging(Config.SenparcWeixinSetting.WeixinAppId, Convert.ToInt32(ConfigurationManager.AppSettings["UserTagStudent"]), openids);
                }
                else if (staff.Type == "teacher")
                {
                    UserTagApi.BatchTagging(Config.SenparcWeixinSetting.WeixinAppId, Convert.ToInt32(ConfigurationManager.AppSettings["UserTagTeacher"]), openids);

                    //再检查用户的监考信息
                    ExamStaffInvigilateModel esi = ExamStaffInvigilateBll.Instance.Get(staff);
                    if (esi == null)
                    {
                        esi = new ExamStaffInvigilateModel();
                        esi.AutoArranged = 1;
                        esi.Created = DateTime.Now;
                        esi.Invigilated = 0;
                        esi.StaffId = staff.KeyId;
                        esi.Updated = DateTime.Now;

                        ExamStaffInvigilateBll.Instance.Insert(esi);
                    }
                }

                context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
            }
            else if (action == "unbind")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel model = CommonStaffBll.Instance.Get(openid);
                string oldType = model.Type;

                model.Serial = null;
                model.Name = null;
                model.Type = null;

                if (CommonStaffBll.Instance.Update(model) > 0)
                {
                    //给当前用户删除相应的标签
                    List<string> openids = new List<string>();
                    openids.Add(openid);

                    if (oldType == "student")
                    {
                        UserTagApi.BatchUntagging(Config.SenparcWeixinSetting.WeixinAppId, Convert.ToInt32(ConfigurationManager.AppSettings["UserTagStudent"]), openids);
                    }
                    else if (oldType == "teacher")
                    {
                        UserTagApi.BatchUntagging(Config.SenparcWeixinSetting.WeixinAppId, Convert.ToInt32(ConfigurationManager.AppSettings["UserTagTeacher"]), openids);
                    }

                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                }
            }

            context.Response.End();
            return;
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