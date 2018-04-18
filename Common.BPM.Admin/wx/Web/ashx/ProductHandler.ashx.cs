using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using BPM.FivePower.Bll;
using BPM.FivePower.Extension;
using BPM.FivePower.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.wx.Web.ashx
{
    /// <summary>
    /// ProductHandler 的摘要说明
    /// </summary>
    public class ProductHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<FivePowerProductModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<FivePowerProductModel>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "registe":
                    FivePowerProductModel model = FivePowerProductBll.Instance.Get(rpm.Entity.DepartmentId, rpm.Entity.Serial);
                    if (model != null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "product_has_registed" }));
                    }
                    else
                    {
                        model = rpm.Entity;
                        model.InstallTime = DateTime.Now;
                        model.FinishedTime = DateTime.Now.AddYears(3);
                        model.FinishedDriving = model.Driving.Value + 100000;
                        model.Memo = "";

                        if (FivePowerProductBll.Instance.Insert(model) > 0)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = true}));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "saved_error" }));
                        }
                    }
                    break;
                case "params":
                    context.Response.Write(JSONhelper.ToJson(
                        new
                        {
                            Addresses = DepartmentBll.Instance.Get(rpm.KeyId).Address.Split(';'),
                            Models = DicBll.Instance.GetListBy("cpxh").Select(d => new { Title = d.Title, Code = d.Code }).ToArray()
                        }));
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