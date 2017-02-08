using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Washer.BasePage
{
    public class AuthorizeBasePage : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            string deptId = Request.Params["appid"];
            if (string.IsNullOrEmpty(deptId))
            {
                Response.Redirect("/PublicPlatform/Web/Error.html?c=1");
            }

            Department dept = DepartmentBll.Instance.Get(Convert.ToInt32(deptId));
            string code = Request.Params["code"];
            if (string.IsNullOrEmpty(code))
            {
                //没有经过验证过来的
                string url = OAuthApi.GetAuthorizeUrl(dept.Appid, Request.Url.ToString(), "0", Senparc.Weixin.MP.OAuthScope.snsapi_base, "code");
                Response.Redirect(url);
            }
            else
            {
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(dept.Appid, dept.Secret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    Response.Redirect("/PublicPlatform/Web/Error.html?c=2");
                }
                else
                {
                    Session["deptId"] = dept.KeyId;
                    Session["openid"] = result.openid;
                }
            }
        }
    }
}
