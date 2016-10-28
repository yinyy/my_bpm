using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.Admin.Extra
{
    /// <summary>
    /// ValidateUser 的摘要说明
    /// </summary>
    public class ValidateUser : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Write("success,PICC");
            context.Response.Flush();
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