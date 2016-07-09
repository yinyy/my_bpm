using BPM.Common;
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
    /// WasherCardHandler 的摘要说明
    /// </summary>
    public class WasherCardHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCardModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCardModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherCardModel model;
            string filter;
            switch (rpm.Action)
            {
                case "add":
                    model = rpm.Entity;
                    WasherCardModel c2 = WasherCardBll.Instance.Get(departmentId, model.CardNo);
                    if (c2 == null)
                    {
                        model.DepartmentId = departmentId;
                        model.Kind = "temporary";
                        model.Memo = "";
                        model.Coins *= 100;

                        context.Response.Write(WasherCardBll.Instance.Add(model));
                    }
                    else
                    {
                        context.Response.Write("-1");
                    }
                    break;
                case "del":
                    context.Response.Write(WasherCardBll.Instance.Delete(rpm.KeyId));
                    break;
                case "consume":
                    filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"CardId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"lt\",\"data\":\"0\"}}],\"groups\":[]}}", rpm.KeyId);
                    context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                case "recharge":
                    filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"CardId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"gt\",\"data\":\"0\"}}],\"groups\":[]}}", rpm.KeyId);
                    context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}",departmentId, rpm.Filter);
                        context.Response.Write(WasherCardBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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