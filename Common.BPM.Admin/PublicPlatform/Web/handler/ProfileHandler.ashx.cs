using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Omu.ValueInjecter;
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
    /// ProfileHandler 的摘要说明
    /// </summary>
    public class ProfileHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string action = context.Request.Params["action"];
            string appid = context.Session["appid"].ToString();
            string openid = context.Session["openid"].ToString();

            Department dept = DepartmentBll.Instance.GetByAppid(appid);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            WasherConsumeModel consume ;

            if (action == "bind")
            {
                string name = context.Request.Params["name"];
                string gender = context.Request.Params["gender"];
                string telphone = context.Request.Params["telphone"];
                string password = context.Request.Params["password"];
                string vcode = context.Request.Params["vcode"];

                WasherVcodeModel code = WasherVcodeBll.Instance.Get(telphone);
                if (code == null || code.Validated != null||code.Vcode!=vcode)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "验证码错误。" }));
                }
                else if (code.Created.AddMinutes(3) < DateTime.Now)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "验证码已过期。" }));
                }
                else
                {
                    code.Validated = DateTime.Now;
                    WasherVcodeBll.Instance.Update(code);

                    consume = WasherConsumeBll.Instance.Get(dept.KeyId, telphone);
                    #region 不存在用户信息的时候，直接绑定
                    if (consume == null)
                    {
                        consume = new WasherConsumeModel();
                        consume.Bindered = DateTime.Now;
                        consume.BinderId = wxconsume.KeyId;
                        consume.Memo = "";
                        consume.Points = 0;
                        consume.DepartmentId = dept.KeyId;
                        consume.Gender = gender;
                        consume.Telphone = telphone;
                        consume.Password = password;
                        consume.Name = name;

                        if ((consume.KeyId = WasherConsumeBll.Instance.Add(consume)) > 0)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "绑定用户错误。" }));
                        }
                    }
                    #endregion
                    #region 已经有绑定的用户
                    else if (consume.BinderId != null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "用户已经被绑定。" }));
                    }
                    #endregion
                    else
                    {
                        consume.Name = name;
                        consume.Password = password;
                        consume.Gender = gender;
                        consume.Bindered = DateTime.Now;
                        consume.BinderId = wxconsume.KeyId;

                        if (WasherConsumeBll.Instance.Update(consume) > 0)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { Success = true, Message = "绑定用户错误。" }));
                        }
                    }
                }
            }
            else if (action == "unbind")
            {
                consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

                if (consume != null)
                {
                    consume.BinderId = null;
                    consume.Bindered = null;
                    if (WasherConsumeBll.Instance.Update(consume) > 0)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "解除绑定时发生错误。" }));
                    }
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "用户不存在。" }));
                }
            }
            else
            {
                WeixinUserInfoResult userInfo = CommonApi.GetUserInfo(AccessTokenContainer.GetAccessToken(appid), openid);
                consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

                if (wxconsume == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false }));
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = true, Binded = consume != null, Image = userInfo.headimgurl, Nickname = userInfo.nickname }));
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