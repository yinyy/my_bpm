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
    /// MenuData 的摘要说明
    /// </summary>
    public class MenuData : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            if (!SysVisitor.Instance.IsGuest)
            {
                var userName = SysVisitor.Instance.UserName;
                var menuJSON = "var menus = " + UserBll.Instance.GetNavJson(userName);
                context.Response.Write(menuJSON);
            }
            else
            {
                context.Response.Write("var menus = -1;"); //没有登录
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