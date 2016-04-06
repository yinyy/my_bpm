using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Washer.Bll;
using Washer.Model;
using WeChat.Utils;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherHandler 的摘要说明
    /// </summary>
    public class WasherHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request.Params["action"];
            switch (action)
            {
                default:
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