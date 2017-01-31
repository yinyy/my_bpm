using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Data;
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
    public class WasherDeviceLogHandler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();

            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherDeviceLogModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherDeviceLogModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter;
            switch (rpm.Action)
            {
                case "export":
                    if (user.IsAdmin)
                    {
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherDeviceLogBll.Instance.Export(rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherDeviceLogBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                    }
                    break;
                case "balance":
                    WasherDeviceLogModel balance = WasherDeviceLogBll.Instance.Get(rpm.KeyId);
                    int ticks = (int)(DateTime.Now - DateTime.Parse("1970-01-01")).TotalSeconds;

                    if (balance.CardId == 0)/*这代表的是微信支付*/
                    {
                        balance.PayCoins = rpm.Entity.PayCoins;
                    }
                    else
                    {
                        balance.PayCoins = WasherCardBll.Instance.Deduction(balance.CardId, rpm.Entity.PayCoins, ticks);
                    }
                    balance.Ticks = ticks;
                    balance.Ended = DateTime.Now;

                    if (WasherDeviceLogBll.Instance.Update(balance) < 0)
                    {
                        context.Response.Write(-1);
                    }
                    else
                    {
                        context.Response.Write(balance.KeyId);
                    }

                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}",departmentId, rpm.Filter);
                        context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                    }
                    break;
            }

            //context.Response.Flush();
            //context.Response.End();
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