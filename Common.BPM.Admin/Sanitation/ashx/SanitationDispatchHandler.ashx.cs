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

            //int k;
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
                    SanitationDispatchModel d = new SanitationDispatchModel();
                    d.InjectFrom(rpm.Entity);
                    if (d.Enabled == null)
                    {
                        d.Enabled = "否";
                    }

                    if (d.Enabled == "是"){
                        if (SanitationDispatchBll.Instance.Validate(d))
                        {
                            context.Response.Write(SanitationDispatchBll.Instance.Add(d)); ;
                        }
                        else
                        {
                            context.Response.Write("0");
                        }
                    }
                    else
                    {
                        context.Response.Write(SanitationDispatchBll.Instance.Add(d)); ;
                    }
                    break;
                case "edit":
                    d = new SanitationDispatchModel();
                    d.InjectFrom(rpm.Entity);
                    d.KeyId = rpm.KeyId;
                    if (d.Enabled == null)
                    {
                        d.Enabled = "否";
                    }

                    if (d.Enabled == "是")
                    {
                        if (SanitationDispatchBll.Instance.Validate(d))
                        {
                            context.Response.Write(SanitationDispatchBll.Instance.Update(d));
                        }
                        else
                        {
                            context.Response.Write("0");
                        }
                    }
                    else
                    {
                        context.Response.Write(SanitationDispatchBll.Instance.Update(d));
                    }
                    break;
                case "delete":
                    context.Response.Write(SanitationDispatchBll.Instance.Delete(rpm.KeyId));
                    break;
                case "enable":
                    d = SanitationDispatchBll.Instance.GetById(rpm.KeyId);
                    d.Enabled = "是";

                    if (SanitationDispatchBll.Instance.Validate(d))
                    {
                        context.Response.Write(SanitationDispatchBll.Instance.Update(d));
                    }
                    else
                    {
                        context.Response.Write("0");
                    }
                    break;
                case "analyse_card":
                    d = SanitationDispatchBll.Instance.GetById(rpm.KeyId);

                    if (d != null)
                    {
                        SanitationDriverModel dm = SanitationDriverBll.Instance.GetById(d.DriverId);
                        SanitationTrunkModel tm = SanitationTrunkBll.Instance.GetById(d.TrunkId);

                        var obj = new
                        {
                            Name = dm.Name,
                            Code = dm.Code,
                            Plate = tm.Plate,
                            Workload = d.Workload,
                            Time = d.Time.ToString("yyyy年MM月dd日"),
                            Finished = SanitationDetailBll.Instance.Get(d.KeyId).Count()
                        };

                        context.Response.Write(JSONhelper.ToJson(obj));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(d));
                    }
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