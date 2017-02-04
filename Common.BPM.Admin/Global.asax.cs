using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;
using Combres;
using BPM.Core.Bll;
using Senparc.Weixin.MP.CommonAPIs;
using BPM.Core.Model;
using System.Threading;
using SuperSocket.WebSocket;
using Newtonsoft.Json;
using Washer.Bll;

namespace BPM.Admin
{
    public class Global : System.Web.HttpApplication
    {
        private static ManualResetEvent resetEvent = new ManualResetEvent(false);

        protected void Application_Start(object sender, EventArgs e)
        {
            RouteTable.Routes.AddCombresRoute("Combres");

            //注册所有的公众号的appid和secret
            foreach (Department d in DepartmentBll.Instance.GetAll())
            {
                if (!string.IsNullOrWhiteSpace(d.Appid))
                {
                    AccessTokenContainer.Register(d.Appid, d.Secret);
                }
            }
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            //在出现未处理的错误时运行的代码
            Exception erroy = Server.GetLastError();
            string err = "出错页面：" + Request.Url.ToString() + "</br>";
            err += "异常信息：" + erroy.Message + "</br>";
            //err += "Source:" + erroy.Source + "</br>";
            //err += "StackTrace:" + erroy.StackTrace + "</br>";
            //清除前一个异常
            Server.ClearError();

            //此处理用Session["ProError"]出错。所以用 Application["ProError"]
            Application["error"] = err;
            //此处不是page中，不能用Response.Redirect("../frmSysError.aspx");
            HttpContext.Current.Response.Redirect("~/ApplicationError.aspx");
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}