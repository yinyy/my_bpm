using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Web.SessionState;
using System.Text;

namespace BPM.Admin.Sanitation.ashx
{
    /// <summary>
    /// SanitationWorkRegionHandler 的摘要说明
    /// </summary>
    public class SanitationWorkRegionHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];

            switch (action)
            {
                case "save":
                    string text = string.Format("var workRegionPoints = {0};", context.Request.Params["path"]);
                    string file = context.Server.MapPath("~/Sanitation/js/SanitationMapData.js");
                    using (StreamWriter sw = new StreamWriter(file,false))
                    {
                        sw.Write(text);
                    }

                    context.Response.Write("success");
                    break;
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