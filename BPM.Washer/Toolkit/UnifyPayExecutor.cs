using BPM.Common;
using BPM.Core.Bll;
using BPM.Core.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Containers;
using System;
using System.Linq;
using System.Threading;
using Washer.Bll;
using Washer.Model;
using WebSocket4Net;

namespace Washer.Toolkit
{
    public class UnifyPayExecutor
    {
        /// <summary>
        /// 处理成功支付的订单
        /// </summary>
        /// <param name="serial">本地订单编号</param>
        public static void Execute(string serial)
        {
            WasherOrderModel order = WasherOrderBll.Instance.Get(serial);
            if (order.Kind == "购买洗车卡")
            {
                HandleBuyCard(order);
            }
            else if (order.Kind == "微信支付洗车")
            {
                HandlePayWash(order);
            }
        }

        private static void HandlePayWash(WasherOrderModel order)
        {
            new Thread(() =>
            {
                var info = new { Desc = "", Board = "" };
                info = JsonConvert.DeserializeAnonymousType(order.Memo, info);

                Department dept = DepartmentBll.Instance.Get(order.DepartmentId);
                WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(WasherWeChatConsumeBll.Instance.Get(order.DepartmentId, order.OpenId));
                WasherDeviceModel device = WasherDeviceBll.Instance.Get(order.DepartmentId, info.Board);

                //将支付信息写入设备日志
                WasherDeviceLogModel balance = new WasherDeviceLogModel();
                balance.CardId = 0;
                balance.ConsumeId = consume == null ? -1 : consume.KeyId;
                balance.DeviceId = device.KeyId;
                balance.Kind = "微信支付";
                balance.Memo = order.Serial;
                balance.PayCoins = 0;
                balance.RemainCoins = order.Money;
                balance.Started = DateTime.Now;
                balance.IsShow = true;

                balance.KeyId = WasherDeviceLogBll.Instance.Add(balance);

                var o = new
                {
                    Action = "start_machine",
                    Data = JsonConvert.SerializeObject(new
                    {
                        DepartmentId = dept.KeyId,
                        BoardNumber = Aes.Encrypt(info.Board),
                        BalanceId = balance.KeyId,
                        Coins = balance.RemainCoins
                    })
                };

                WebSocket webSocket = new WebSocket("ws://139.129.43.203:5500");
                webSocket.Opened += (s0, e0) =>
                {
                    webSocket.Send(JsonConvert.SerializeObject(o));
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), order.OpenId,
                        string.Format("洗车机已经启动，支付：{0:0.00}元。会员洗车更优惠！", balance.RemainCoins / 100.0));

                    try { webSocket.Close(); } catch { }
                };
                webSocket.Error += (s0, e0) =>
                {
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), order.OpenId, "微信支付成功。洗车机启动时发生异常，请联系客服。");
                    try { webSocket.Close(); } catch { }
                };
                webSocket.Open();

                WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);
                int point = 0;
                #region 微信支付洗车送积分
                if ((point = order.Money * setting.PayWashCar.Wx / 100 / 100) > 0)
                {
                    WasherRewardModel reward = new WasherRewardModel();
                    reward.ConsumeId = consume.KeyId;
                    reward.Expired = false;
                    reward.Kind = "微信支付洗车送积分";
                    reward.Memo = JSONhelper.ToJson(new { OrderId = order.KeyId, Money = string.Format("{0:0.00}", order.Money / 100.0) });
                    reward.Points = point;
                    reward.Time = DateTime.Now;
                    reward.Used = 0;

                    reward.KeyId = WasherRewardBll.Instance.Add(reward);
                }
                #endregion

                #region 推荐奖励送积分
                int level = 1;
                foreach (var pst in setting.GiftLevel)
                {
                    WasherWeChatConsumeModel wxconsume = null;
                    point = 0;
                    if (consume != null
                    && consume.BinderId != null
                    && (wxconsume = WasherWeChatConsumeBll.Instance.Get(consume.BinderId.Value)) != null
                    && wxconsume.RefererId > 0
                    && (wxconsume = WasherWeChatConsumeBll.Instance.Get(wxconsume.RefererId)) != null
                    && (consume = WasherConsumeBll.Instance.GetByBinderId(wxconsume.KeyId)) != null
                    && (point = order.Money * pst / 100 / 100) > 0)
                    {
                        WasherRewardModel reward = new WasherRewardModel();
                        reward.ConsumeId = consume.KeyId;
                        reward.Expired = false;
                        reward.Kind = "推荐奖励送积分";
                        reward.Memo = JSONhelper.ToJson(new { OrderId = order.KeyId, Money = string.Format("{0:0.00}", order.Money / 100.0), Level = level, Percent = pst, Desc = "微信支付洗车" });
                        reward.Points = point;
                        reward.Time = DateTime.Now;
                        reward.Used = 0;

                        reward.KeyId = WasherRewardBll.Instance.Add(reward);

                        level++;
                    }
                }
                #endregion
            }).Start();
        }

        private static void HandleBuyCard(WasherOrderModel order)
        {
            new Thread(() =>
            {
                var info = new { Desc = "", Card = "" };
                info = JsonConvert.DeserializeAnonymousType(order.Memo, info);

                //解密卡号
                string card = Aes.Decrypt(info.Card);

                Department dept = DepartmentBll.Instance.Get(order.DepartmentId);
                WasherConsumeModel consume = WasherConsumeBll.Instance.GetByBinder(WasherWeChatConsumeBll.Instance.Get(order.DepartmentId, order.OpenId));
                #region 绑定洗车卡成功
                if (WasherCardBll.Instance.Bind(consume, card))
                {
                    #region 通知用户
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), order.OpenId, string.Format("洗车卡购买成功，卡号：{0}，已于您的账户绑定。", card));
                    #endregion

                    WasherDepartmentSetting setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);
                    #region 买卡送积分
                    var os = new { Desc = "", Card = "" };
                    os = JsonConvert.DeserializeAnonymousType(order.Memo, os);

                    WasherDepartmentSettingBuyCardOption buyCardOption = null;
                    foreach (var p in setting.BuyCardOption)
                    {
                        if (p.Product == os.Desc)
                        {
                            buyCardOption = p;
                            break;
                        }
                    }

                    if (buyCardOption != null && order.ConsumeId != null)
                    {
                        WasherRewardModel reward = new WasherRewardModel();
                        reward.ConsumeId = order.ConsumeId.Value;
                        reward.Expired = false;
                        reward.Kind = "买卡送积分";
                        reward.Memo = JSONhelper.ToJson(new { OrderId = order.KeyId, ConsumeId = consume.KeyId, ConsumeName = consume.Name });
                        reward.Points = buyCardOption.Score;
                        reward.Time = DateTime.Now;
                        reward.Used = 0;

                        reward.KeyId = WasherRewardBll.Instance.Add(reward);
                    }
                    #endregion

                    #region 推荐奖励送积分
                    int level = 1;
                    foreach (var pst in setting.GiftLevel)
                    {
                        WasherWeChatConsumeModel wxconsume = null;
                        int point = 0;
                        if (consume != null
                            && consume.BinderId != null
                            && (wxconsume = WasherWeChatConsumeBll.Instance.Get(consume.BinderId.Value)) != null
                            && wxconsume.RefererId > 0
                            && (wxconsume = WasherWeChatConsumeBll.Instance.Get(wxconsume.RefererId)) != null
                            && (consume = WasherConsumeBll.Instance.GetByBinderId(wxconsume.KeyId)) != null
                            && (point = order.Money * pst / 100 / 100) > 0)
                        {
                            WasherRewardModel reward = new WasherRewardModel();
                            reward.ConsumeId = consume.KeyId;
                            reward.Expired = false;
                            reward.Kind = "推荐奖励送积分";
                            reward.Memo = JSONhelper.ToJson(new { OrderId = order.KeyId, Money = string.Format("{0:0.00}", order.Money / 100.0), Level = level, Percent = pst, Desc = "购买洗车卡" });
                            reward.Points = point;
                            reward.Time = DateTime.Now;
                            reward.Used = 0;

                            reward.KeyId = WasherRewardBll.Instance.Add(reward);

                            level++;
                        }
                    }
                    #endregion
                }
                #endregion
                #region 绑定洗车卡失败
                else
                {
                    #region 通知用户
                    CustomApi.SendText(AccessTokenContainer.TryGetAccessToken(dept.Appid, dept.Secret), order.OpenId, "洗车卡购买成功。绑定洗车卡时发生错误，请联系客服。");
                    #endregion
                }
                #endregion
            }).Start();
        }
    }
}