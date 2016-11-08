using BPM.Agriculture.Bll;
using BPM.Common;
using BPM.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPM.Admin.Agriculture.ashx
{
    /// <summary>
    /// AgricultureMapHandler 的摘要说明
    /// </summary>
    public class AgricultureMapHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string action = context.Request.Params["action"];

            switch (action)
            {
                case "add":
                    
                    break;
                case "edit":
                    
                    break;
                case "delete":
                    
                    break;
                default:
                    context.Response.Write(JSONhelper.ToJson(AgricultureDeviceBll.Instance.GetAll().ToList()));
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