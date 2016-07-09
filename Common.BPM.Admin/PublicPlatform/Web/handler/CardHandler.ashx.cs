using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Washer.Bll;
using Washer.Model;
using BPM.Common;
using Newtonsoft.Json.Linq;
using BPM.Core.Model;
using BPM.Core.Bll;
using Newtonsoft.Json;
using System.Web.SessionState;
using Washer.Extension;

namespace BPM.Admin.PublicPlatform.Web.handler
{
    /// <summary>
    /// CardHandler 的摘要说明
    /// </summary>
    public class CardHandler : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string action = context.Request.Params["action"];
            string appid = context.Session["appid"].ToString();
            string openid = context.Session["openid"].ToString();

            Department dept = DepartmentBll.Instance.GetByAppid(appid);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

            if (action == "list")
            {
                var q = WasherCardBll.Instance.GetValidCards(consume.KeyId).Select(a => new { No = a.CardNo, Coins = a.Coins, ValidateFrom = string.Format("{0:yyyy-MM-dd}", a.ValidateFrom), ValidateEnd = string.Format("{0:yyyy-MM-dd}", a.ValidateEnd) }).ToArray();
                context.Response.Write(JSONhelper.ToJson(new { count = q.Count(), data = q }));
            }
            else if (action == "bind")
            {
                string no = context.Request.Params["card"];
                string password = context.Request.Params["password"];

                WasherCardModel card = WasherCardBll.Instance.Get(dept.KeyId, no);
                if (card == null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "洗车卡不存在。" }));
                }
                else if (card.Password != password)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "密码错误。" }));
                }
                else if (card.BinderId != null)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "洗车卡已被其他用户绑定。" }));
                }
                else if (DateTime.Now > card.ValidateEnd)
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "洗车卡以过期。" }));
                }
                else
                {
                    card.BinderId = consume.KeyId;
                    card.Binded = DateTime.Now;
                    if (WasherCardBll.Instance.Update(card) > 0)
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                    }
                    else
                    {
                        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "洗车卡绑定失败。" }));
                    }
                }
            }
            else if (action == "query")
            {
                int value = Convert.ToInt32(context.Request.Params["value"]);
                int remain = WasherCardBll.Instance.GetCardCountByValue(dept.KeyId, value * 100);
                float price = 0;
                string product = "";

                JObject jobj = JObject.Parse(dept.Setting);
                JArray array = jobj.GetValue("Buy") as JArray;
                foreach (JObject o in array)
                {
                    if (Convert.ToInt32(o.GetValue("Card")) == value)
                    {
                        price = Convert.ToSingle(o.GetValue("Price"));
                        product = o.GetValue("Product").ToString();

                        break;
                    }
                }
                context.Response.Write(JSONhelper.ToJson(new { Success = true, Remain = remain, Price = price, Product = product }));
            }
            else if (action == "payBind")
            {
                int value = Convert.ToInt32(context.Request.Params["value"]);
                if (WasherCardBll.Instance.Bind(consume, value * 100))
                {
                    //增加积分
                    JObject jobj = JObject.Parse(dept.Setting);
                    JArray array = jobj.GetValue("Buy") as JArray;
                    foreach (JObject o in array)
                    {
                        if (Convert.ToInt32(o.GetValue("Card")) == value)
                        {
                            int score = Convert.ToInt32(o.GetValue("Score"));

                            WasherRewardModel reward = new WasherRewardModel()
                            {
                                ConsumeId = consume.KeyId,
                                Kind = WasherRewardBll.Kind.BuyCard,
                                Memo = "",
                                Points = score,
                                Time = DateTime.Now
                            };
                            WasherRewardBll.Instance.Add(reward);

                            break;
                        }
                    }

                    context.Response.Write(JSONhelper.ToJson(new { Success = true }));
                }
                else
                {
                    context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "绑定洗车卡失败。" }));
                }
            }
            else
            {
                context.Response.Write(JSONhelper.ToJson(new { Appid = appid, Openid = openid }));
            }

            context.Response.Flush();
            context.Response.End();

            //int wxid = Convert.ToInt32(context.Request.Params["wxid"]);
            //WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinderId(wxid);
            //var deptSetting = new
            //{
            //    Subscribe = 0,
            //    Recharge = new int[3],
            //    PointKind = "",
            //    Level = new int[5],
            //    Buy = new object[4]
            //};
            
            //switch (action)
            //{
            //    case "query":
            //        {
            //            Department dept = DepartmentBll.Instance.Get(consume.DepartmentId);
            //            int value = Convert.ToInt32(context.Request.Params["value"]);
            //            int count = WasherCardBll.Instance.GetCardCountByValue(consume.DepartmentId, value * 100);
            //            float price = 0;
            //            string product = "";

            //            JObject jobj = JObject.Parse(dept.Setting);
            //            JArray array = jobj.GetValue("Buy") as JArray;
            //            foreach (JObject o in array)
            //            {
            //                if (Convert.ToInt32(o.GetValue("Card")) == value)
            //                {
            //                    price = Convert.ToSingle(o.GetValue("Price"));
            //                    product = o.GetValue("Product").ToString();

            //                    break;
            //                }
            //            }
            //            context.Response.Write(JSONhelper.ToJson(new { Success = true, Count = count, Price = price, Product = product }));
            //        }

            //        break;
            //    case "bind":
            //        string cardNo = context.Request.Params["CardNo"];
            //        string password = context.Request.Params["Password"];
            //        WasherCardModel card = WasherCardBll.Instance.Get(consume.DepartmentId, cardNo);
            //        if (card == null)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { Success = 0, Message = "洗车卡不存在。" }));
            //        }
            //        else if (card.Password != password)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { Success = 0, Message = "密码错误。" }));
            //        }
            //        else if (card.BinderId != null)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { Success = 0, Message = "洗车卡已被其他用户绑定。" }));
            //        }
            //        else if (DateTime.Now > card.ValidateEnd)
            //        {
            //            context.Response.Write(JSONhelper.ToJson(new { Success = 0, Message = "洗车卡以过期。" }));
            //        }
            //        else
            //        {
            //            card.BinderId = consume.KeyId;
            //            card.Binded = DateTime.Now;
            //            if (WasherCardBll.Instance.Update(card) > 0)
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { Success = 1, Message = "洗车卡绑定成功。" }));
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { Success = 0, Message = "洗车卡绑定失败。" }));
            //            }
            //        }
            //        break;
            //    case "payBind":
            //        {
            //            Department dept = DepartmentBll.Instance.Get(consume.DepartmentId);
            //            int value = Convert.ToInt32(context.Request.Params["value"]);
            //            if (WasherCardBll.Instance.Bind(consume, value * 100))
            //            {
            //                //增加积分
            //                JObject jobj = JObject.Parse(dept.Setting);
            //                JArray array = jobj.GetValue("Buy") as JArray;
            //                foreach (JObject o in array)
            //                {
            //                    if (Convert.ToInt32(o.GetValue("Card")) == value)
            //                    {
            //                        int score = Convert.ToInt32(o.GetValue("Score"));

            //                        WasherRewardModel reward = new WasherRewardModel()
            //                        {
            //                            ConsumeId = consume.KeyId,
            //                            Kind = WasherRewardBll.Kind.BuyCard,
            //                            Memo = "",
            //                            Points = score,
            //                            Time = DateTime.Now
            //                        };
            //                        WasherRewardBll.Instance.Add(reward);

            //                        break;
            //                    }
            //                }
                            
            //                context.Response.Write(JSONhelper.ToJson(new { Success = true }));
            //            }
            //            else
            //            {
            //                context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "绑定洗车卡失败。" }));
            //            }
            //        }
            //        break;
            //    default:
            //        var q = WasherCardBll.Instance.GetValidCards(consume.KeyId).OrderBy(a => a.ValidateEnd);
            //        context.Response.Write(JSONhelper.ToJson(new { count = q.Count(), data = q.ToList() }));
            //        break;
            //}

            //context.Response.End();
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