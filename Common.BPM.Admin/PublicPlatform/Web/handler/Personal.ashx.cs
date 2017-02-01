using BPM.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// Personal 的摘要说明
    /// </summary>
    public class Personal : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];
            int consumeId = Convert.ToInt32(context.Session["consumeId"]);

            WasherConsumeModel consume = WasherConsumeBll.Instance.Get(consumeId);
            switch (action)
            {
                case "save":
                    var o = new { MaxPayCoins = Convert.ToInt32(context.Request["max_pay_coins"]) * 100 };

                    consume.Setting = JSONhelper.ToJson(o);
                    context.Response.Write(WasherConsumeBll.Instance.Update(consume));
                    break;
                default:
                    context.Response.Write(consume.Setting);
                    break;
            }

            context.Response.Flush();
            context.Response.Close();
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