using BPM.Core.Bll;
using BPM.Core.Model;
using System;
using System.Configuration;
using Washer.Bll;
using Washer.Model;
using WeChat.Model;
using WeChat.Processor;
using WeChat.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using BPM.Common;

namespace Washer.Processor
{
    public class WasherProcessor : MessageProcessor
    {
        public void process(ReceivedMessageModel received)
        {
            //if (received.Type == MessageType.Event)
            //{
            //    #region 订阅事件
            //    if (received.Event == EventType.Subscribe)
            //    {
            //        ReplyTextPictureMessageModel message = new ReplyTextPictureMessageModel() { To = received.From, From = received.To };
            //        message.ArticleCount = 1;
            //        message.Articles = new ReplyTextPictureMessageModel.TextPictureItem[] { new ReplyTextPictureMessageModel.TextPictureItem {
            //            Description = "欢迎加入自助洗车大家庭！",
            //            Title="自助洗车大家庭",
            //            PictureUrl="http://www.dyxy.net/images/qcode.png",
            //            Url="http://www.baidu.com"
            //        } };

            //        WeChatToolkit.ReplyMessage(message);

                    
            //    }
            //    #endregion
            //    #region 扫码等待事件
            //    else if (received.Event == EventType.ScanCodeWaitMsg)
            //    {
            //        if (received.Key == "m111_wash_car")
            //        {
            //            //返回成功，微信客户端不需要重发
            //            WeChatToolkit.ReplyMessage();

            //            #region 通过序列号得到设备
            //            string serialNumber = received.ScanCodeInfo.Result;
            //            WasherDeviceModel device = WasherDeviceBll.Instance.Get(serialNumber);
            //            if (device == null)
            //            {
            //                return;
            //            }
            //            #endregion

            //            #region 检查用户是否具有使用这台洗车机的权限
            //            WasherConsumeModel consume = WasherConsumeBll.Instance.Get(device.DepartmentId);
            //            if (consume == null)
            //            {
            //                WeChatToolkit.PostMessage(JSONhelper.ToJson(new
            //                {
            //                    touser = received.From,
            //                    msgtype = "text",
            //                    text = new
            //                    {
            //                        content = "您不能使用这台洗车机。请先关注。"
            //                    }
            //                }));

            //                return;
            //            }
            //            #endregion

            //            #region 检查设备的工作状态
            //            if (device.Status != "就绪")
            //            {
            //                WeChatToolkit.PostMessage(JSONhelper.ToJson(new
            //                {
            //                    touser = received.From,
            //                    msgtype = "text",
            //                    text = new
            //                    {
            //                        content = "洗车机正在工作，请稍后。"
            //                    }
            //                }));

            //                return;
            //            }
            //            #endregion

            //            #region 检查用户的洗车币余额
            //            var deviceSetting = new { Coin = 0.0f };
            //            deviceSetting = JsonConvert.DeserializeAnonymousType(device.Setting, deviceSetting);
            //            //if (consume.Coins < deviceSetting.Coin)
            //            //{
            //            //    WeChatToolkit.PostMessage(JSONhelper.ToJson(new
            //            //    {
            //            //        touser = received.From,
            //            //        msgtype = "text",
            //            //        text = new
            //            //        {
            //            //            content = "您的余额已不足，请先充值。"
            //            //        }
            //            //    }));

            //            //    return;
            //            //}
            //            #endregion

            //            #region 扣费、返积分
            //            Department dept = DepartmentBll.Instance.Get(device.DepartmentId);
            //            var departmentSetting = new { Point = new { WashCar = 0 } };//{"WashCar":10,"Subscribe":1,"Recharge":[2,3,4],"PointKind":"Percent","Level":[5,6,7,8,9]}
            //            departmentSetting = JsonConvert.DeserializeAnonymousType(dept.Setting, departmentSetting);

            //            //consume.Coins -= deviceSetting.Coin;
            //            consume.Points += departmentSetting.Point.WashCar;
            //            if (WasherConsumeBll.Instance.Update(consume) <= 0)
            //            {
            //                WeChatToolkit.PostMessage(JSONhelper.ToJson(new
            //                {
            //                    touser = received.From,
            //                    msgtype = "text",
            //                    text = new
            //                    {
            //                        content = "扣费失败，请稍后重试。"
            //                    }
            //                }));

            //                return;
            //            }

            //            WasherSpendingModel spending = new WasherSpendingModel();
            //            spending.Coins = deviceSetting.Coin;
            //            spending.ConsumeId = consume.KeyId;
            //            spending.DeviceId = device.KeyId;
            //            spending.Kind = WasherSpendingBll.Kind.WashCar;
            //            spending.Memo = "";
            //            spending.Time = DateTime.Now;
            //            WasherSpendingBll.Instance.Add(spending);

            //            WasherRewardModel reward = new WasherRewardModel();
            //            reward.ConsumeId = consume.KeyId;
            //            reward.Kind = WasherRewardBll.Kind.WashCar;
            //            reward.Memo = "";
            //            reward.Points = departmentSetting.Point.WashCar;
            //            reward.Time = DateTime.Now;
            //            WasherRewardBll.Instance.Add(reward);
            //            #endregion

            //            #region 设备开始工作

            //            #endregion

            //            #region 通知用户
            //            WeChatToolkit.PostMessage(JSONhelper.ToJson(new
            //            {
            //                touser = received.From,
            //                msgtype = "text",
            //                text = new
            //                {
            //                    content = string.Format("设备准备就绪。\n您本次消费 {0} 洗车币，还有 {1} 洗车币，本次新增 {2} 积分，累计 {3} 积分。",
            //                    deviceSetting.Coin, 
            //                    //consume.Coins,
            //                    departmentSetting.Point.WashCar,
            //                    consume.Points)
            //                }
            //            }));
            //            #endregion
            //        }
            //        else if (received.Key == "m121_card")
            //        {//点击会员卡菜单项
            //            ReplyTextMessageModel message = new ReplyTextMessageModel() { To = received.From, From = received.To };
            //            message.Content = string.Format("当前时间clk：{0}", DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"));

            //            WeChatToolkit.ReplyMessage(message);
            //        }
            //        else if (received.Key == "m123_promote")
            //        {//点击了推广菜单
            //            //ReplyTextMessageModel model = new ReplyTextMessageModel() { To = received.From, From = received.To };
            //            //model.Content = "向好友介绍，请点 <a href=\"http://139.129.43.203/PublicPlatform/Web/Promote.aspx?on=00xx00x\">这里</a> 。";

            //            ReplyTextPictureMessageModel model = new ReplyTextPictureMessageModel() { To = received.From, From = received.To };
            //            model.ArticleCount = 1;
            //            model.Articles = new ReplyTextPictureMessageModel.TextPictureItem[] {
            //                new ReplyTextPictureMessageModel.TextPictureItem {
            //                    Description = "向好友介绍我们的自助洗车服务，<br/><br/>还可以享受到很多积分哟。",
            //                    PictureUrl="http://139.129.43.203/PublicPlatform/Web/images/transparent_1x1.png",
            //                    Title="向好友介绍",
            //                    Url="http://139.129.43.203/PublicPlatform/Web/Promote.aspx?on=00xx00x"
            //                }
            //            };

            //            WeChatToolkit.ReplyMessage(model);
            //        }
            //    }
            //    #endregion
            //}
        }

        private int ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
    }
}