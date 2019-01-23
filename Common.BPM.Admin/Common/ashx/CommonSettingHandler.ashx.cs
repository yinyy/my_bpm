using BPM.Common;
using BPM.Core;
using BPM.Core.Dal;
using BPM.Core.Model;
using Course.Common.Bll;
using Course.Common.Model;
using Newtonsoft.Json;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Common.ashx
{
    /// <summary>
    /// CommonSettingHandler 的摘要说明
    /// </summary>
    public class CommonSettingHandler : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var json = context.Request["json"];
            var rpm = new RequestParamModel<CommonSettingModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<CommonSettingModel>>(json);
                rpm.CurrentContext = context;
            }

            User currentUser = UserDal.Instance.Get(SysVisitor.Instance.UserId);
            CommonSettingModel settings;

            switch (rpm.Action)
            {
                case "js":
                    settings = CommonSettingBll.Instance.Get(currentUser.DepartmentId);
                    if (settings == null)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = settings }));
                    }
                    break;
                case "save":
                    settings = CommonSettingBll.Instance.Get(currentUser.DepartmentId);
                    if (settings == null)
                    {
                        settings = new CommonSettingModel();
                    }
                    settings.InjectFrom(rpm.Entity);
                    settings.DepartmentId = currentUser.DepartmentId;

                    if((settings.KeyId == 0?CommonSettingBll.Instance.Insert(settings):CommonSettingBll.Instance.Update(settings))>0)
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
                    }
                    else
                    {
                        context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                    }
                    break;
                default:
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