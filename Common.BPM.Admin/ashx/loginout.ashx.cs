using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using BPM.Core;
using BPM.Core.Bll;

namespace BPM.Admin.ashx
{
    /// <summary>
    /// loginout 的摘要说明
    /// </summary>
    public class loginout : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            
            SysVisitor.Instance.LoginOut();

            context.Response.Write("ok");
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