using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using Omu.ValueInjecter;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Common;
using Sanitation.Model;
using Sanitation.Bll;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class SanitationDriverHandler : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<SanitationDriverModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<SanitationDriverModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "add":
                    context.Response.Write(SanitationDriverBll.Instance.Add(rpm.Entity));
                    break;
                case "edit":
                    SanitationDriverModel d = new SanitationDriverModel();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    context.Response.Write(SanitationDriverBll.Instance.Update(d));
                    break;
                case "delete":
                    context.Response.Write(SanitationDriverBll.Instance.Delete(rpm.KeyId));
                    break;
                case "combobox":
                    if (context.Request.Params["showCode"] == "true")
                    {
                        context.Response.Write(JSONhelper.ToJson(SanitationDriverBll.Instance.GetAll().Select(ad => new { KeyId = ad.KeyId, Title = ad.Name+"["+ad.Code+"]" }).OrderBy(ad => ad.Title)));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(SanitationDriverBll.Instance.GetAll().Select(ad => new { KeyId = ad.KeyId, Title = ad.Name }).OrderBy(ad => ad.Title)));
                    }
                    break;
                case "get":
                    d = SanitationDriverBll.Instance.GetById(rpm.KeyId);
                    context.Response.Write(d);
                    break;
                default:
                    context.Response.Write(SanitationDriverBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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