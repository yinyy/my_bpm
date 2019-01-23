using Course.Common.Bll;
using Course.Common.Model;
using Newtonsoft.Json;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.AdvancedAPIs.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform.ashx
{
    /// <summary>
    /// OAuth2Handler 的摘要说明
    /// </summary>
    public class OAuth2Handler : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request["action"];
            if (action == "openid")
            {
                if (context.Session["openid"] != null)
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = true}));
                }
                else
                {
                    context.Response.Write(JsonConvert.SerializeObject(new { Success = false }));
                }
            }
            else if (action == "base")
            {
                string code = context.Request["code"];
                if (string.IsNullOrEmpty(code))
                {
                    context.Response.Write("您拒绝了授权！");
                    context.Response.End();
                    return;
                }

                string state = context.Request["state"];
                if (state != context.Session["State"] as string)
                {
                    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
                    //建议用完之后就清空，将其一次性使用
                    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                    context.Response.Write("验证失败！请从正规途径进入！");
                    context.Response.End();
                    return;
                }

                //通过，用code换取access_token
                OAuthAccessTokenResult result = OAuthApi.GetAccessToken(Config.SenparcWeixinSetting.WeixinAppId,Config.SenparcWeixinSetting.WeixinAppSecret, code);
                if (result.errcode != ReturnCode.请求成功)
                {
                    context.Response.Write("错误：" + result.errmsg);
                    context.Response.End();
                    return;
                }

                //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
                //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
                context.Session["OAuthAccessTokenStartTime"] = SystemTime.Now;
                context.Session["OAuthAccessToken"] = result;

                string nextUrl = context.Request["nextUrl"];

                //因为这里还不确定用户是否关注本微信，所以只能试探性地获取一下
                OAuthUserInfo userInfo = null;
                try
                {
                    //已关注，可以得到详细信息
                    userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                    #region 获取用户相关信息
                    #endregion

                    if (!string.IsNullOrEmpty(nextUrl))
                    {
                        context.Response.Redirect(nextUrl);
                        context.Response.End();
                        return;
                    }
                }
                catch 
                {
                    //未关注，只能授权，无法得到详细信息
                    //这里的 ex.JsonResult 可能为："{\"errcode\":40003,\"errmsg\":\"invalid openid\"}"
                    //context.Response.Write("用户已授权，授权Token：" + result);
                    context.Response.Write("用户已授权，但无法获取用户信息。");
                    context.Response.End();
                    return;
                }
            }
            else if (action == "userinfo")
            {
                string code = context.Request["code"];
                if (string.IsNullOrEmpty(code))
                {
                    context.Response.Write("您拒绝了授权！");
                    context.Response.End();
                    return;
                }

                string state = context.Request["state"];
                if (state != context.Session["State"] as string)
                {
                    //这里的state其实是会暴露给客户端的，验证能力很弱，这里只是演示一下，
                    //建议用完之后就清空，将其一次性使用
                    //实际上可以存任何想传递的数据，比如用户ID，并且需要结合例如下面的Session["OAuthAccessToken"]进行验证
                    context.Response.Write("验证失败！请从正规途径进入！");
                    context.Response.End();
                    return;
                }

                OAuthAccessTokenResult result = null;
                //通过，用code换取access_token
                try
                {
                    result = OAuthApi.GetAccessToken(Config.SenparcWeixinSetting.WeixinAppId, Config.SenparcWeixinSetting.WeixinAppSecret, code);
                }
                catch (Exception ex)
                {
                    context.Response.Write(ex.Message);
                    context.Response.End();
                    return;
                }

                if (result.errcode != ReturnCode.请求成功)
                {
                    context.Response.Write("错误：" + result.errmsg);
                    context.Response.End();
                    return;
                }

                //下面2个数据也可以自己封装成一个类，储存在数据库中（建议结合缓存）
                //如果可以确保安全，可以将access_token存入用户的cookie中，每一个人的access_token是不一样的
                context.Session["OAuthAccessTokenStartTime"] = SystemTime.Now;
                context.Session["OAuthAccessToken"] = result;

                string nextUrl = context.Request["nextUrl"];

                //因为第一步选择的是OAuthScope.snsapi_userinfo，这里可以进一步获取用户详细信息
                try
                {
                    OAuthUserInfo userInfo = OAuthApi.GetUserInfo(result.access_token, result.openid);

                    context.Session["openid"] = userInfo.openid;

                    #region 正常进入下一级页面
                    if (!string.IsNullOrEmpty(nextUrl))
                    {
                        context.Response.Redirect(nextUrl);
                        context.Response.End();
                        return;
                    }
                    #endregion
                }
                catch (ErrorJsonResultException ex)
                {
                    context.Response.Write(ex.Message);
                    context.Response.End();
                    return;
                }
            }
            else
            {
                #region 向微信服务器发起用户认证的请求
                string state = "Course-" + SystemTime.Now.Millisecond;//随机数，用于识别请求可靠性
                context.Session.Add("State", state);//储存随机数到Session

                string nextUrl = context.Request["nextUrl"];
                string returnUrl = string.Format("http://course.dyzyxyydwlwsys.cc/PublicPlatform/ashx/OAuth2Handler.ashx?action=userinfo&nextUrl={0}", nextUrl.UrlEncode());
                string oauthUrl = OAuthApi.GetAuthorizeUrl(Config.SenparcWeixinSetting.WeixinAppId, returnUrl, state, OAuthScope.snsapi_userinfo);
                #endregion

                context.Response.Write(JsonConvert.SerializeObject(new { Success = true, Data = oauthUrl }));
                //context.Response.Redirect(oauthUrl);
                context.Response.End();
                return;
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