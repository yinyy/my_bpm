using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Extension;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// AuthorizeHandler 的摘要说明
    /// </summary>
    public class AuthorizeHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string code = context.Request.Params["code"];
            int deptId = Convert.ToInt32(context.Request.Params["appid"]);
            Department dept = DepartmentBll.Instance.Get(deptId);

            if (string.IsNullOrEmpty(code))
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = true, Appid=dept.Appid }));
            }
            else
            {
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(dept.Appid, dept.Secret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false }));
                }
                else
                {
                    context.Session["deptId"] = dept.KeyId;
                    context.Session["openid"] = result.openid;

                    WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, result.openid);
                    WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinderId(wxconsume.KeyId);

                    context.Session["consumeId"] = consume.KeyId;

                    //context.Session["appid"] = "wx2d8bcab64b53be3a";
                    //context.Session["openid"] = "oiVK2uH3zgJLC6iGMoB6iuDKDW1M";

                    context.Response.Write(JSONhelper.ToJson(new { Success = true, Openid = result.openid }));
                    //WeixinUserInfoResult userInfo = CommonApi.GetUserInfo(AccessTokenContainer.GetAccessToken(dept.Appid), result.openid);
                    //context.Response.Write(JSONhelper.ToJson(new { Success = true, Openid = userInfo.openid }));
                }
            }

            context.Response.Flush();
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