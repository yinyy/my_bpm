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
                    string data = context.Request.Params["data"];
                    string[] ds = data.Split(',');
                    int driverId = Convert.ToInt32(ds[2].Substring(0, ds[2].IndexOf('[')));
                    int trunkId = Convert.ToInt32(ds[3].Substring(0, ds[3].IndexOf('[')));

                    var obj = new
                    {
                        Name = SanitationDriverBll.Instance.GetById(driverId).Name,
                        Code = ds[2].Substring(ds[2].IndexOf('[')+1, ds[2].IndexOf(']') - ds[2].IndexOf('[') - 1),
                        Plate = SanitationTrunkBll.Instance.GetById(trunkId).Plate,
                        Workload = ds[4],
                        Time = ds[0]
                    };

                    context.Response.Write(JSONhelper.ToJson(obj));
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