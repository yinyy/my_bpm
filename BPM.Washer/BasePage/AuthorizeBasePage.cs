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
using Washer.Bll;
using Washer.Model;

namespace Washer.BasePage
{
    public class AuthorizeBasePage : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["consumeId"] == null)
            {
                string deptId = Session["deptId"] == null ? Request.Params["appid"] : Session["deptId"].ToString();
                if (string.IsNullOrEmpty(deptId))
                {
                    Response.Redirect("/PublicPlatform/Web/Error.html?c=1");
                }

                Department dept = DepartmentBll.Instance.Get(Convert.ToInt32(deptId));
                string code = Request.Params["code"];
                if (string.IsNullOrEmpty(code))
                {
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

                        WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, result.openid);
                        WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinderId(wxconsume.KeyId);

                        if (consume != null)
                        {
                            Session["consumeId"] = consume.KeyId;
                        }
                    }
                }
            }
        }
    }
}
