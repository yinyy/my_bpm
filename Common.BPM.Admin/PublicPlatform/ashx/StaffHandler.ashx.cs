using Course.Common.Bll;
using Course.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                string name = string.Format("{1}[{0}]", serial, type == "student" ? "学生" : "教工");

                var obj = new { Success = true, Data = new { Serial = serial, Name = name, Type = type } };
                context.Response.Write(JsonConvert.SerializeObject(obj));
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
                string openid = context.Session["openid"].ToString();
                string serial = context.Request["serial"];
                string name = context.Request["name"];
                string type = context.Request["type"];

                CommonStaffModel model = CommonStaffBll.Instance.Get(openid);
                model.Serial = serial;
                model.Name = name;
                model.Type = type;

                if (CommonStaffBll.Instance.Update(model) > 0)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                }
            }
            else if (action == "unbind")
            {
                string openid = context.Session["openid"].ToString();
                CommonStaffModel model = CommonStaffBll.Instance.Get(openid);
                model.Serial = null;
                model.Name = null;
                model.Type = null;

                if (CommonStaffBll.Instance.Update(model) > 0)
                {
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