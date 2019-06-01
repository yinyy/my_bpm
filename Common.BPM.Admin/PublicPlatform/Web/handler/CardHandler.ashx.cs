using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.SessionState;
using Washer.Bll;
using Washer.Model;
using Washer.Toolkit;

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
            int deptId=Convert.ToInt32(context.Session["deptId"].ToString());
            string openid = context.Session["openid"].ToString();

            Department dept = DepartmentBll.Instance.Get(deptId);
            WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(dept.KeyId, openid);
            WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(wxconsume);

            if (action == "list")
            {
                var q = WasherCardBll.Instance.GetValidCards(consume.KeyId).Select(a => new { No = a.CardNo, Coins = a.Coins, ValidateFrom = string.Format("{0:yyyy-MM-dd}", a.ValidateFrom), ValidateEnd = string.Format("{0:yyyy-MM-dd}", a.ValidateEnd) }).ToArray();
                context.Response.Write(JSONhelper.ToJson(new { binded = true, count = q.Count(), data = q }));
            }
            else if (action == "bind")
            {
                string no = context.Request.Params["card"];
                string password = context.Request.Params["password"];
                string vcode = context.Request.Params["vcode"];
                string telphone = context.Request.Params["telphone"];

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
                if (string.IsNullOrEmpty(context.Request.Params["value"]))
                {
                    WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);

                    JArray array = new JArray();
                    foreach (WasherDepartmentSettingBuyCardOption b in setting.BuyCardOption)
                    {
                        JObject jobj = new JObject();
                        jobj.Add("Value", b.Value);
                        jobj.Add("Price", b.Price);
                        jobj.Add("Remain", WasherCardBll.Instance.GetCardCountByValue(dept.KeyId, b.Value * 100));
                        jobj.Add("Product", b.Product);

                        array.Add(jobj);
                    }

                    context.Response.Write(JSONhelper.ToJson(new { Success = true, Data = array }));
                }
                else
                {
                    int value = Convert.ToInt32(context.Request.Params["value"]);
                    int remain = WasherCardBll.Instance.GetCardCountByValue(dept.KeyId, value * 100);
                    float price = 0;
                    string product = "";

                    JObject jobj = JObject.Parse(dept.Setting);
                    JArray array = jobj.GetValue("Buy") as JArray;
                    foreach (JObject o in array)
                    {
                        if (Convert.ToInt32(o.GetValue("Value")) == value)
                        {
                            price = Convert.ToSingle(o.GetValue("Price"));
                            product = o.GetValue("Product").ToString();

                            break;
                        }
                    }
                    context.Response.Write(JSONhelper.ToJson(new { Success = true, Remain = remain, Price = price, Product = product }));
                }
            }
            //else if (action == "payBind")
            //{
            //    int value = Convert.ToInt32(context.Request.Params["value"]);
            //    if (WasherCardBll.Instance.Bind(consume, value * 100))
            //    {
            //        //增加积分
            //        JObject jobj = JObject.Parse(dept.Setting);
            //        JArray array = jobj.GetValue("Buy") as JArray;
            //        foreach (JObject o in array)
            //        {
            //            if (Convert.ToInt32(o.GetValue("Value")) == value)
            //            {
            //                int score = Convert.ToInt32(o.GetValue("Score"));
            //                //积分大于0时再增加积分
            //                if (score > 0)
            //                {
            //                    WasherRewardModel reward = new WasherRewardModel()
            //                    {
            //                        ConsumeId = consume.KeyId,
            //                        Kind = WasherRewardBll.Kind.BuyCard,
            //                        Memo = "",
            //                        Points = score,
            //                        Time = DateTime.Now
            //                    };
            //                    WasherRewardBll.Instance.Add(reward);
            //                }
            //                break;
            //            }
            //        }

            //        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
            //    }
            //    else
            //    {
            //        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "绑定洗车卡失败。" }));
            //    }
            //}
            //else if (action == "payBind2")
            //{
            //    int value = Convert.ToInt32(context.Request.Params["value"]);
            //    string card = context.Request.Params["card"];
            //    #region 解密卡号
            //    card = Aes.Decrypt(card);
            //    #endregion
            //    if (WasherCardBll.Instance.Bind(consume, card))
            //    {
            //        //增加积分
            //        JObject jobj = JObject.Parse(dept.Setting);
            //        JArray array = jobj.GetValue("Buy") as JArray;
            //        foreach (JObject o in array)
            //        {
            //            if (Convert.ToInt32(o.GetValue("Value")) == value)
            //            {
            //                int score = Convert.ToInt32(o.GetValue("Score"));
            //                //积分大于0时再增加积分
            //                if (score > 0)
            //                {
            //                    WasherRewardModel reward = new WasherRewardModel()
            //                    {
            //                        ConsumeId = consume.KeyId,
            //                        Kind = WasherRewardBll.Kind.BuyCard,
            //                        Memo = "",
            //                        Points = score,
            //                        Time = DateTime.Now
            //                    };
            //                    WasherRewardBll.Instance.Add(reward);
            //                }
            //                break;
            //            }
            //        }

            //        context.Response.Write(JSONhelper.ToJson(new { Success = true }));
            //    }
            //    else
            //    {
            //        context.Response.Write(JSONhelper.ToJson(new { Success = false, Message = "绑定洗车卡失败。" }));
            //    }
            //}
            else if (action == "telphone")
            {
                context.Response.Write(JSONhelper.ToJson(new { Success = true, Binded = consume != null, Message = consume != null ? consume.Telphone : "" }));
            }
            else if (action == "lock")
            {
                LockCardInfo lci = new LockCardInfo(consume.KeyId, Convert.ToInt32(context.Request.Params["value"]));
                LoopThread.Add(lci);

                while (!lci.Finished)
                {
                    Thread.Sleep(200);
                }

                context.Response.Write(lci.CardNumber);
            }
            else
            {
                context.Response.Write(JSONhelper.ToJson(new { Appid = dept.Appid, Openid = openid }));
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