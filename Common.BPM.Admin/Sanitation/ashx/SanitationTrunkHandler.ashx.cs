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
    public class SanitationTrunkHandler : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            //int k;
            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<SanitationTrunkModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<SanitationTrunkModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "add":
                    SanitationTrunkModel d = new SanitationTrunkModel();
                    d.InjectFrom(rpm.Entity);
                    d.Plate = d.Plate.ToUpper();

                    context.Response.Write(SanitationTrunkBll.Instance.Add(d));
                    break;
                case "edit":
                    d = new SanitationTrunkModel();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    d.Plate = d.Plate.ToUpper();

                    context.Response.Write(SanitationTrunkBll.Instance.Update(d));
                    break;
                case "get":
                    d = SanitationTrunkBll.Instance.GetById(rpm.KeyId);
                    context.Response.Write(JSONhelper.ToJson(d));
                    break;
                case "delete":
                    context.Response.Write(SanitationTrunkBll.Instance.Delete(rpm.KeyId));
                    break;
                case "combobox":
                    context.Response.Write(JSONhelper.ToJson(SanitationTrunkBll.Instance.GetAll().Select(ad => new { KeyId = ad.KeyId, Title = ad.Plate }).OrderBy(ad=>ad.Title)));
                    break;
                
                default:
                    context.Response.Write(SanitationTrunkBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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