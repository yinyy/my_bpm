using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core.Bll;
using BPM.Common;
using BPM.Core;
using BPM.Core.Model;
using Omu.ValueInjecter;
using WeChat.Utils;

namespace BPM.Admin.sys.ashx
{
    /// <summary>
    /// Summary description for DepartmentHandler
    /// </summary>
    public class DepartmentHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<Department>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<Department>>(json);
                rpm.CurrentContext = context;
            }
            
            switch (rpm.Action)
            {
                case "add":
                    context.Response.Write(DepartmentBll.Instance.AddNewDepartment(rpm.Entity));
                    break;
                case "edit":
                    Department d = new Department();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    context.Response.Write(DepartmentBll.Instance.EditDepartment(d));
                    break;
                case "delete":
                    context.Response.Write(DepartmentBll.Instance.DeleteDepartment(rpm.KeyId));
                    break;
                case "subscribe_qrcode":
                    Department dept = DepartmentBll.Instance.Get(rpm.KeyId);

                    string ticket = "";
                    if (dept.Qrticket == null)
                    {
                        dept.Qrticket = ticket = WeChatQrcodeHelper.GetPermanenceCode(rpm.KeyId);
                        DepartmentBll.Instance.Update(dept);
                    }
                    else
                    {
                        ticket = dept.Qrticket;
                    }

                    context.Response.Write(JSONhelper.ToJson(new { Success = !string.IsNullOrWhiteSpace(ticket), Ticket = ticket }));
                    break;
                default:
                    context.Response.Write(DepartmentBll.Instance.GetDepartmentTreegridData());
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