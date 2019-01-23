using Course.Common.Model;
using Course.Core.Bll;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.Course.ashx
{
    /// <summary>
    /// BranchHandler 的摘要说明
    /// </summary>
    public class BranchHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];

            if (action == "oauth")
            {
                if (context.Session["staff"] == null)
                {
                    var obj = new { Success=false, Url = "http://course.dyzyxyydwlwsys.cc/PublicPlatform/ashx/OAuth2Handler.ashx?type=student&nextUrl=http://course.dyzyxyydwlwsys.cc/PublicPlatform/Course/Branch.aspx" };
                    context.Response.Write(JsonConvert.SerializeObject(obj));
                }
                else
                {
                    var obj = new { Success = true };
                    context.Response.Write(JsonConvert.SerializeObject(obj));
                }
            }
            else if (action == "detail")
            {
                context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = CourseBranchBll.Instance.Get(Convert.ToInt32(context.Request["id"])) }));
            }
            else if(action=="list")
            {
                context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = CourseBranchBll.Instance.FindAll() }));
            }

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