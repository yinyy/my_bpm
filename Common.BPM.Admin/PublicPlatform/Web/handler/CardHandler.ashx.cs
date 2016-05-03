using BPM.Common;
using BPM.Core;
using Omu.ValueInjecter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Washer.Bll;
using Washer.Model;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// CardHandler 的摘要说明
    /// </summary>
    public class CardHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            var json = HttpContext.Current.Request["json"];
            var rpm = new RequestParamModel<WasherCardModel>(context) { CurrentContext = context };
            if (!string.IsNullOrEmpty(json))
            {
                rpm = JSONhelper.ConvertToObject<RequestParamModel<WasherCardModel>>(json);
                rpm.CurrentContext = context;
            }

            WasherCardModel card;
            switch (rpm.Action)
            {
                case "password":
                    card = WasherCardBll.Instance.Get(rpm.Entity.DepartmentId, rpm.Entity.CardNo);
                    if (card == null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 1 }));
                    }
                    else if (card.BinderId != null && card.BinderId != rpm.Entity.BinderId)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 2 }));
                    }
                    else if (card.Kind == "temporary" && (DateTime.Now < card.ValidateFrom || DateTime.Now > card.ValidateEnd))
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 3 }));
                    }
                    else
                    {
                        #region 将密码发送至用户手机
                        WasherConsumeModel consume = WasherConsumeBll.Instance.Get(rpm.Entity.BinderId.Value);
                        #endregion 
                        context.Response.Write(JSONhelper.ToJson(new { success = 0 }));
                    }
                    break;
                case "bind":
                    card = WasherCardBll.Instance.Get(rpm.Entity.DepartmentId, rpm.Entity.CardNo);
                    if (card == null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 1 }));
                    }else if (card.Password != rpm.Entity.Password)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 2 }));
                    }
                    else if (card.BinderId != null && card.BinderId != rpm.Entity.BinderId)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 3 }));
                    }
                    else if (card.Kind == "temporary" && (DateTime.Now < card.ValidateFrom || DateTime.Now > card.ValidateEnd))
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 4 }));
                    }
                    else
                    {
                        WasherConsumeModel consume = WasherConsumeBll.Instance.Get(rpm.Entity.BinderId.Value);
                        card.BinderId = consume.KeyId;
                        card.Binded = DateTime.Now;

                        if (WasherCardBll.Instance.Update(card)>0)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { success = 0 }));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { success = 5 }));
                        }                        
                    }
                    break;
                case "unbind":
                    card = WasherCardBll.Instance.Get(rpm.KeyId);
                    if (card == null)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { success = 1 }));
                    }else
                    {
                        card.BinderId = null;
                        card.Binded = null;
                        if (WasherCardBll.Instance.Update(card) > 0)
                        {
                            context.Response.Write(JSONhelper.ToJson(new { success = 0 }));
                        }
                        else
                        {
                            context.Response.Write(JSONhelper.ToJson(new { success = 2 }));
                        }
                    }
                    break;
                default:
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