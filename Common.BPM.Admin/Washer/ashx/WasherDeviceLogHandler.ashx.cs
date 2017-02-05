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

            bool isDepartmentAdmin = false;
            foreach (Role r in user.Roles)
            {
                if (r.RoleName == "大客户管理员")
                {
                    isDepartmentAdmin = true;
                    break;
                }
            }

            string filter;
            switch (rpm.Action)
            {
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
                case "show":
                    if(user.IsAdmin || isDepartmentAdmin)
                    {
                        WasherDeviceLogModel dl = WasherDeviceLogBll.Instance.Get(rpm.KeyId);
                        dl.IsShow = !dl.IsShow;

                        context.Response.Write(WasherDeviceLogBll.Instance.Update(dl));
                    }else
                    {
                        context.Response.Write(-1);
                    }
                    break;
                case "export":
                    if (user.IsAdmin)
                    {
                        GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherDeviceLogBll.Instance.Export(rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        if (isDepartmentAdmin) {
                            filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                            GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherDeviceLogBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                        }
                        else
                        {
                            filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}},{{\"field\":\"IsShow\",\"op\":\"eq\",\"data\":\"1\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                            GridViewExportUtil.Export(DateTime.Now.ToString("yyyyMMddHHmmssffff") + ".xls", WasherDeviceLogBll.Instance.Export(filter, rpm.Sort, rpm.Order));
                        }
                    }
                    break;
                default:
                    if (user.IsAdmin)
                    {
                        context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, rpm.Filter, rpm.Sort, rpm.Order));
                    }
                    else
                    {
                        if (isDepartmentAdmin)
                        {
                            filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                            context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                        }else
                        {
                            filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"DepartmentId\",\"op\":\"eq\",\"data\":\"{0}\"}}, {{\"field\":\"IsShow\",\"op\":\"eq\",\"data\":\"1\"}}],\"groups\":[{1}]}}", departmentId, rpm.Filter);
                            context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, rpm.Sort, rpm.Order));
                        }
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