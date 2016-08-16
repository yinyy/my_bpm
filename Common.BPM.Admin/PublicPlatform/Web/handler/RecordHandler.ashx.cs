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
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// RecordHandler 的摘要说明
    /// </summary>
    public class RecordHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string appid = context.Session["appid"].ToString();
            string openid = context.Session["openid"].ToString();

            Department dept = DepartmentBll.Instance.GetByAppid(appid);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherDeviceLogModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherDeviceLogModel>>(json);
                rpm.CurrentContext = context;
            }

            string filter = string.Format("{{\"groupOp\":\"AND\",\"rules\":[{{\"field\":\"ConsumeId\",\"op\":\"eq\",\"data\":\"{0}\"}}],\"groups\":[]}}", consume.KeyId);
            string fields = " Started, Kind, convert(decimal(5,2), PayCoins/100.0) as PayCoins, Address ";
            context.Response.Write(WasherDeviceLogBll.Instance.GetJson(rpm.Pageindex, rpm.Pagesize, filter, "Started", "Desc", fields));

            context.Response.Flush();
            context.Response.End();
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