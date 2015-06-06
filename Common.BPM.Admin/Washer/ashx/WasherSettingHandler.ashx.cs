using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherSetting 的摘要说明
    /// </summary>
    public class WasherSettingHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();
            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            WasherSettingModel model;
            string action = context.Request.Params["action"];
            switch (action)
            {
                case "js":
                    model = WasherSettingBll.Instance.GetByDepartmentId(departmentId);
                    if (model == null)
                    {
                        context.Response.Write("var config={money: 0, other: ''}");
                    }
                    else
                    {
                        context.Response.Write("var config = " + model.Value);
                    }
                    break;
                default:
                    model = WasherSettingBll.Instance.GetByDepartmentId(departmentId);
                    if (model == null)
                    {
                        model = new WasherSettingModel();
                        model.DepartmentId = departmentId;
                        model.Value = "";

                        model.KeyId = WasherSettingBll.Instance.Add(model);
                    }

                    model.Value = context.Request.Params["json"];

                    context.Response.Write(WasherSettingBll.Instance.Update(model));

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