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

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}