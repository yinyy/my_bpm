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
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message= "用户已经被绑定。" }));
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
                        context.Response.Write(JSONhelper.ToJson(new { Success = true}));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true, Message= "绑定用户错误。" }));
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


            //var json = HttpContext.Current.Request["json"];
            //var rpm = new RequestParamModel<WasherConsumeModel>(context) { CurrentContext = context };
            //if (!string.IsNullOrEmpty(json))
            //{
            //    rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherConsumeModel>>(json);
            //    rpm.CurrentContext = context;
            //}

            //WasherConsumeModel consume;

            //switch (rpm.Action)
            //{
            //    case "vcode":
            //        //获得有效的验证码
            //        WasherVcodeModel vcode = WasherVcodeBll.Instance.Get(rpm.Entity.Telphone);
            //        if (vcode == null || vcode.Validated != null || vcode.Created.AddMinutes(3) <= DateTime.Now)
            //        #region 没有该手机号对应的验证码，或验证码已经过期
            //        {
            //            vcode = new WasherVcodeModel();
            //            vcode.Created = DateTime.Now;
            //            vcode.Memo = "";
            //            vcode.Validated = null;
            //            vcode.Telphone = rpm.Entity.Telphone;
            //            vcode.Vcode = string.Format("{0:000000}", DateTime.Now.Ticks % 1000000);

            //            if (WasherVcodeBll.Instance.Save(vcode) > 0)
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 1 }));

            //                //TODO 发送验证码到手机号
            //                #region 发送验证码到手机号
            //                #endregion
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 0 }));
            //            }
            //        }
            //        #endregion
            //        else
            //        #region 发送已有的验证码
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { success = 1 }));

            //            //TODO 发送验证码到手机号
            //            #region 发送验证码到手机号
            //            #endregion
            //        }
            //        #endregion
            //        break;
            //    case "bind":
            //        WasherVcodeResult code = WasherVcodeBll.Instance.Validate(rpm.Entity.Telphone, rpm.Entity.Vcode, 3);
            //        #region 判断用户的验证码是否正确                    
            //        if (code != WasherVcodeResult.验证码正确)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = code.ToString() }));
            //            return;
            //        }
            //        #endregion
                    
            //        consume = WasherConsumeBll.Instance.Get(rpm.Entity.DepartmentId, rpm.Entity.Telphone);
            //        #region 不存在用户信息的时候，直接绑定
            //        if (consume == null)
            //        {
            //            consume = rpm.Entity;
            //            consume.Bindered = DateTime.Now;
            //            consume.Memo = "";
            //            consume.Points = 0;

            //            if ((consume.KeyId = WasherConsumeBll.Instance.Add(consume)) > 0)
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 1, keyId = consume.KeyId }));
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = "绑定用户错误。" }));
            //            }
            //        }
            //        #endregion
            //        #region 已经有绑定的用户
            //        else if (consume.BinderId != null)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = "用户已经被绑定。" }));
            //        }
            //        #endregion
            //        else
            //        {
            //            consume.Name = rpm.Entity.Name;
            //            consume.Password = rpm.Entity.Password;
            //            consume.Gender = rpm.Entity.Gender;
            //            consume.Bindered = DateTime.Now;
            //            consume.BinderId = rpm.Entity.BinderId;

            //            if (WasherConsumeBll.Instance.Update(consume) > 0)
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 1, keyId = consume.KeyId }));
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = "绑定用户错误。" }));
            //            }
            //        }
            //        break;
            //    case "unbind":
            //        consume = WasherConsumeBll.Instance.Get(rpm.KeyId);
            //        if (consume != null)
            //        {
            //            consume.BinderId = null;
            //            consume.Bindered = null;

            //            if (WasherConsumeBll.Instance.Update(consume) > 0)
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 1, keyId = consume.KeyId }));
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = "解除绑定时发生错误。" }));
            //            }
            //        }
            //        else
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { success = 0, errmsg = "用户不存在。" }));
            //        }
            //        break;
            //    default:




            //        break;
            //}
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