using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using BPM.FivePower.Extension;
using Newtonsoft.Json;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.FivePower.ashx
{
    /// <summary>
    /// FivePowerSetting 的摘要说明
    /// </summary>
    public class FivePowerSettingHandler : IHttpHandler, IRequiresSessionState
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
                    dept.Address= context.Request.Params["Address"];

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