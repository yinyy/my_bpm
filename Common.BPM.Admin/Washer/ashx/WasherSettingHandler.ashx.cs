using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using Senparc.Weixin.MP.CommonAPIs;

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherSetting 的摘要说明
    /// </summary>
    public class WasherSettingHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();
            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;
         
            Department dept;
            string action = context.Request.Params["action"];
            switch (action)
            {
                //case "menu":
                //    dept = DepartmentBll.Instance.Get(departmentId);
                //    string menu = context.Server.MapPath("~/Washer/js/menu.txt");
                //    StringBuilder sb = new StringBuilder();
                //    using(StreamReader reader = new StreamReader(menu))
                //    {
                //        string line;
                //        while ((line = reader.ReadLine()) != null)
                //        {
                //            sb.Append(line);
                //        }
                //    }

                //    menu = string.Format(sb.ToString(), dept.Appid);

                //    var result = CommonApi.CreateMenu(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), menu);


                //    break;
                case "js":
                    dept = DepartmentBll.Instance.Get(departmentId);
                    context.Response.Write("var json = " + JsonConvert.SerializeObject(dept));

                    break;
                default:
                    dept = DepartmentBll.Instance.Get(departmentId);
                    dept.Appid = context.Request.Params["Appid"];
                    dept.Secret = context.Request.Params["Secret"];
                    dept.Aeskey = context.Request.Params["Aeskey"];
                    dept.Token = context.Request.Params["Token"];
                    dept.Brand = context.Request.Params["Brand"];
                    dept.Logo = context.Request.Params["Logo"];
                    dept.CardColor = context.Request.Params["CardColor"];
                    dept.Introduction = context.Request.Params["Introduction"];
                    dept.Setting = context.Request.Params["Setting"];
                    
                    context.Response.Write(DepartmentBll.Instance.Update(dept));

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