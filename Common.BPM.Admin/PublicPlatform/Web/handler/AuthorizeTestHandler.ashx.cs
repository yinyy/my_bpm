using BPM.Common;
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
            context.Session["deptId"] = "70";
            context.Session["openid"] = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M";

            context.Response.Write(JSONhelper.ToJson(new { Success = true, Openid = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M" }));

            context.Response.Flush();
            context.Response.End();
        }
     
        public bool IsReusable {
            get {
                return false;
            }
        }
    }
}