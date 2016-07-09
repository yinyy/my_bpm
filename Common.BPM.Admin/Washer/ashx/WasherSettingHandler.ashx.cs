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

namespace BPM.Admin.Washer.ashx
{
    /// <summary>
    /// WasherSetting 的摘要说明
    /// </summary>
    public class WasherSettingHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            //var settings = new
            //{
            //    Appid="",
            //    Secret="",
            //    Aeskey="",
            //    Token="",
            //    Brand = "",
            //    Logo = "",
            //    CardColor = "",
            //    Introduction = "",
            //    Setting = new
            //    {
            //        Subscribe = 0,
            //        Recharge = new int[3],
            //        PointKind = "",
            //        Level = new int[5],
            //        Buy=new object[4]
            //    }
            //};

            context.Response.ContentType = "text/plain";

            UserBll.Instance.CheckUserOnlingState();
            User user = SysVisitor.Instance.CurrentUser;
            int departmentId = user.DepartmentId;
         
            Department dept;
            string action = context.Request.Params["action"];
            switch (action)
            {
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