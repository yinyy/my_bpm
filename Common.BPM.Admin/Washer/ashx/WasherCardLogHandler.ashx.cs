using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
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
    /// WasherCardLogHandler 的摘要说明
    /// </summary>
    public class WasherCardLogHandler : IHttpHandler, IRequiresSessionState
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

            string filter;
            switch (rpm.Action)
            {
                //case "cost":
                //    if (user.IsAdmin)
                //    {
                //        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"Coins\",\"op\":\"lt\",\"data\":\"0\"}}],\"groups\":[{0}]}}", rpm.Filter);
                //        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                //    }
                //    else
                //    {
                //        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"lt\",\"data\":\"0\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                //        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                //    }
                //    break;
                //case "income":
                //    if (user.IsAdmin)
                //    {
                //        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"Coins\",\"op\":\"gt\",\"data\":\"0\"}}],\"groups\":[{0}]}}", rpm.Filter);
                //        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                //    }
                //    else
                //    {
                //        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"Coins\",\"op\":\"gt\",\"data\":\"0\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                //        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                //    }
                //    break;
                case "export":
                    if (user.IsAdmin)
                    {
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherCardLogBll.Instance.Export(rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherCardLogBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                    }
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        context.Response.Write(WasherCardLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
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