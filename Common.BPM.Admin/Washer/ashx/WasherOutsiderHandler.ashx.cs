using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherOutsiderHandler 的摘要说明
    /// </summary>
    public class WasherOutsiderHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherOutsiderModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherOutsiderModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherOutsiderModel model;
            string filter;
            switch (rpm.Action)
            {
                case "add":
                    model = WasherOutsiderBll.Instance.Get(user.DepartmentId, rpm.Entity.OutTag);
                    if (model != null)
                    {
                        context.Response.Write(-1);//外部服务重复
                    }
                    else
                    {
                        model = rpm.Entity;
                        model.DepartmentId = user.DepartmentId;

                        context.Response.Write(WasherOutsiderBll.Instance.Add(model));
                    }
                    break;
                case "edit":
                    model = WasherOutsiderBll.Instance.Get(rpm.KeyId);
                    model.Token = rpm.Entity.Token;
                    model.Url = rpm.Entity.Url;
                    model.Memo = rpm.Entity.Memo;

                    context.Response.Write(WasherOutsiderBll.Instance.Update(model));
                    break;
                case "del":
                    context.Response.Write(WasherOutsiderBll.Instance.Delete(rpm.KeyId));
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherOutsiderBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", user.DepartmentId, rpm.Filter);
                        context.Response.Write(WasherOutsiderBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    }
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