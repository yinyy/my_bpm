using BPM.Common;
using BPM.Core;
using Course.Common.Bll;
using Course.Common.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.Platform.ashx
{
    /// <summary>
    /// PlatformSettingHandler 的摘要说明
    /// </summary>
    public class PlatformSettingHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var json = context.Request["json"];
            var rpm = new RequestParamModel<CommonSettingModel[]>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<CommonSettingModel[]>>(json);
                rpm.CurrentContext = context;
            }

            switch (rpm.Action)
            {
                case "js":
                    context.Response.Write(JsonConvert.SerializeObject(new
                    {
                        Success = true,
                        Data = CommonSettingBll.Instance.GetAll().ToArray()
                    }));
                    break;
                case "save":
                    int res = 0;

                    foreach(var s in rpm.Entity)
                    {
                        CommonSettingModel model = CommonSettingBll.Instance.Get(s.Keyword);
                        if (model == null)
                        {
                            model = new CommonSettingModel();
                            model.DepartmentId = 0;
                            model.Keyword = s.Keyword;
                            model.Value = s.Value;

                            res+=CommonSettingBll.Instance.Insert(model);
                        }
                        else
                        {
                            model.Keyword = s.Keyword;
                            model.Value = s.Value;

                            res += CommonSettingBll.Instance.Update(model);
                        }
                    }

                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true }));
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