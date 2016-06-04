using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

using Sanitation.Model;
using Sanitation.Bll;

using Omu.ValueInjecter;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Common;
using BPM.Core.Dal;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// dbHandler 的摘要说明
    /// </summary>
    public class SanitationDispatchHandler : IHttpHandler,IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<SanitationDispatchModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<SanitationDispatchModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "add":
                    SanitationDispatchModel dispatch = new SanitationDispatchModel() {
                        DriverId = rpm.Entity.DriverId,
                        TrunkId = rpm.Entity.TrunkId,
                        Volumn = rpm.Entity.Volumn,
                        Address = DicDal.Instance.GetWhere(new { KeyId = rpm.Entity.Address }).FirstOrDefault().Title,
                        Kind = rpm.Entity.Kind,
                        Potency = rpm.Entity.Potency,
                        Status=0,
                        Time = DateTime.Now,
                        Memo =rpm.Entity.Memo
                    };
                    context.Response.Write(SanitationDispatchBll.Instance.Add(dispatch));
                    break;
                case "delete":
                    context.Response.Write(SanitationDispatchBll.Instance.Delete(rpm.KeyId));
                    break;
                default:
                    context.Response.Write(SanitationDispatchBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
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