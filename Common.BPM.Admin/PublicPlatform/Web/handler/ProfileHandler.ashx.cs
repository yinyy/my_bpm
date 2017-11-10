using BPM.Common;
using BPM.Core;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
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
            string openid = context.Session["openid"].ToString();
            int deptId = Convert.ToInt16(context.Session["deptId"]);

            Department dept = DepartmentBll.Instance.Get(deptId);
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

                    WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);

                    consume = WasherConsumeBll.Instance.Get(dept.KeyId, telphone);
                    #region 不存在用户信息的时候，直接绑定，并分配优惠券
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
                        //consume.Setting = JSONhelper.ToJson(new { MaxPayCoins = 500 });
                        consume.Setting = JSONhelper.ToJson(new { MaxPayCoins = setting.PayWashCar.MaxPayCoins });

                        if ((consume.KeyId = WasherConsumeBll.Instance.Add(consume)) > 0)
                        {
                            string message = "绑定用户成功";
                            
                            if (setting.Register.Coupon > 0)
                            {
                                WasherCardModel card = new WasherCardModel();
                                card.Binded = DateTime.Now;
                                card.BinderId = consume.KeyId;
                                card.CardNo = WasherCardBll.GetNextCouponCardNo(dept.KeyId);
                                card.Coins = setting.Register.Coupon;
                                card.DepartmentId = dept.KeyId;
                                card.Kind = "Coupon";
                                card.Memo = "";
                                card.Password = "123456";
                                card.ValidateFrom = DateTime.Now;
                                card.ValidateEnd = DateTime.Now.AddDays(setting.Register.CouponDay);

                                if (WasherCardBll.Instance.Add(card) > 0)
                                {
                                    message = string.Format("{0}，{1}", message,"赠送优惠卡");
                                }else
                                {
                                    message = string.Format("{0}，{1}", message, "赠送优惠卡失败");
                                }
                            }
                            if (setting.Register.Point > 0)
                            {
                                WasherRewardModel reward = new WasherRewardModel();
                                reward.ConsumeId = consume.KeyId;
                                reward.Expired = false;
                                reward.Kind = "完善个人信息送积分";
                                reward.Memo = "";
                                reward.Points = setting.Register.Point;
                                reward.Time = DateTime.Now;
                                reward.Used = 0;

                                if ((reward.KeyId = WasherRewardBll.Instance.Add(reward)) > 0)
                                {
                                    message = string.Format("{0}，{1}。", message, "增加个人积分");
                                }
                                else
                                {
                                    message = string.Format("{0}，{1}。", message, "增加个人积分失败");
                                }
                            }

                            context.Response.Write(JSONhelper.ToJson(new { Success = true, Message = message }));
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
                WeixinUserInfoResult userInfo = CommonApi.GetUserInfo(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), openid);
                consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

                if (wxconsume == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false }));
                }
                else
                {
                    int coins = consume == null ? 0 : WasherConsumeBll.Instance.GetValidCoins(consume.KeyId);
                    if (coins < 0)
                    {
                        coins = 0;
                    }

                    context.Response.Write(JSONhelper.ToJson(new
                    {
                        Success = true,
                        Binded = consume != null,
                        Image = userInfo.headimgurl,
                        Nickname = userInfo.nickname,
                        Coins = coins/100.0,
                        Reward = consume == null ? 0 : WasherRewardBll.Instance.GetRemainReward(consume.KeyId)
                    }));
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