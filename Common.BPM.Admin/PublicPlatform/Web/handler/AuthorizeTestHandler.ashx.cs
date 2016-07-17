using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// AuthorizeTestHandler 的摘要说明
    /// </summary>
    public class AuthorizeTestHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Session["appid"] = "wx2d8bcab64b53be3a";
            context.Session["openid"] = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M";

            context.Response.Write(JSONhelper.ToJson(new { Success = true, Openid = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M" }));

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