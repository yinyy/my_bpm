using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// Test 的摘要说明
    /// </summary>
    public class Test : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Session.Add("openid", "oiVK2uH3zgJLC6iGMoB6iuDKDW1M");
            context.Session.Add("appid", "wx2d8bcab64b53be3a");
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