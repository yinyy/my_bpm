using BPM.Admin.PublicPlatform.Download;
using BPM.Admin.PublicPlatform.Utilities;
using MP = Senparc.Weixin.MP;
using Senparc.Weixin.MP.Agent;
using Senparc.Weixin.MP.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using BPM.Core.Model;
using BPM.Core.Bll;
using Washer.Model;
using Washer.Bll;
using Newtonsoft.Json;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.AdvancedAPIs;
using System.Threading;

namespace BPM.Admin.PublicPlatform.MessageHandler
{
    public partial class CustomMessageHandler
    {
        private string GetWelcomeInfo()
        {
            //获取Senparc.Weixin.MP.dll版本信息
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(HttpContext.Current.Server.MapPath("~/bin/Senparc.Weixin.MP.dll"));
            var version = string.Format("{0}.{1}", fileVersionInfo.FileMajorPart, fileVersionInfo.FileMinorPart);
            return string.Format(
@"欢迎关注【Senparc.Weixin.MP 微信公众平台SDK】，当前运行版本：v{0}。
您可以发送【文字】【位置】【图片】【语音】等不同类型的信息，查看不同格式的回复。

您也可以直接点击菜单查看各种类型的回复。
还可以点击菜单体验微信支付。

SDK官方地址：http://weixin.senparc.com
源代码及Demo下载地址：https://github.com/JeffreySu/WeiXinMPSDK
Nuget地址：https://www.nuget.org/packages/Senparc.Weixin.MP

===============
更多有关第三方开放平台（Senparc.Weixin.Open）的内容，请回复文字：open
",
                version);
        }

        public string GetDownloadInfo(CodeRecord codeRecord)
        {
            return string.Format(@"您已通过二维码验证，浏览器即将开始下载 Senparc.Weixin SDK 帮助文档。
当前选择的版本：v{0}

我们期待您的意见和建议，客服热线：400-031-8816。

感谢您对盛派网络的支持！

© 2016 Senparc", codeRecord.Version);
        }

        public override IResponseMessageBase OnTextOrEventRequest(RequestMessageText requestMessage)
        {
            // 预处理文字或事件类型请求。
            // 这个请求是一个比较特殊的请求，通常用于统一处理来自文字或菜单按钮的同一个执行逻辑，
            // 会在执行OnTextRequest或OnEventRequest之前触发，具有以下一些特征：
            // 1、如果返回null，则继续执行OnTextRequest或OnEventRequest
            // 2、如果返回不为null，则终止执行OnTextRequest或OnEventRequest，返回最终ResponseMessage
            // 3、如果是事件，则会将RequestMessageEvent自动转为RequestMessageText类型，其中RequestMessageText.Content就是RequestMessageEvent.EventKey

            if (requestMessage.Content == "OneClick")
            {
                var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                strongResponseMessage.Content = "您点击了底部按钮。\r\n为了测试微信软件换行bug的应对措施，这里做了一个——\r\n换行";
                return strongResponseMessage;
            }
            return null;//返回null，则继续执行OnTextRequest或OnEventRequest
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            IResponseMessageBase reponseMessage = null;
            //菜单点击，需要跟创建菜单时的Key匹配
            switch (requestMessage.EventKey)
            {
                default:
                    {
                        var strongResponseMessage = CreateResponseMessage<ResponseMessageText>();
                        strongResponseMessage.Content = "<a href='https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx6dc74da9d1ac4ea4&redirect_uri=http%3a%2f%2f139.129.43.203%2fPublicPlatform%2fWeb%2fCard%2fList.aspx&response_type=code&scope=snsapi_base&state=danis001#wechat_redirect'>这是个超链接，测试一下！</a>";
                        reponseMessage = strongResponseMessage;
                    }
                    break;
            }

            return reponseMessage;
        }

        public override IResponseMessageBase OnEvent_EnterRequest(RequestMessageEvent_Enter requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);
            responseMessage.Content = "您刚才发送了ENTER事件请求。";
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_LocationRequest(RequestMessageEvent_Location requestMessage)
        {
            //这里是微信客户端（通过微信服务器）自动发送过来的位置信息
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这里写什么都无所谓，比如：上帝爱你！";
            return responseMessage;//这里也可以返回null（需要注意写日志时候null的问题）
        }

        public override IResponseMessageBase OnEvent_ScanRequest(RequestMessageEvent_Scan requestMessage)
        {
            ResponseMessageText message = CreateResponseMessage<ResponseMessageText>();
            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                string senceId = requestMessage.EventKey;
                if (senceId.StartsWith("9"))//设备二维码以9开头
                {
                    WasherDeviceModel device;
                    WasherWeChatConsumeModel wxconsume;
                    WasherConsumeModel consume;

                    //如果主板编号错误或者微信用户错误，提示参数错误
                    if ((device = WasherDeviceBll.Instance.Get(deptId, senceId.Substring(1))) == null ||
                        (wxconsume = WasherWeChatConsumeBll.Instance.Get(deptId, requestMessage.FromUserName)) == null)
                    {
                        message.Content = "参数错误！";
                    }
                    //检查该微信用户是否已经登记个人信息，如果没有登记，则提示用户先绑定信息
                    else if ((consume = WasherConsumeBll.Instance.GetByBinder(wxconsume)) == null)
                    {
                        message.Content = "请先在“个人中心-我的账户”中完成个人信息绑定，享受会员权益。";
                    }
                    else {
                        //检查主板是否处于可用状态
                        var setting = JsonConvert.DeserializeAnonymousType(device.Setting, new { Coins = 0, Params = new int[32] });
                        if ((setting.Params[31] & 0x01) == 0x00)
                        {
                            message.Content = "洗车机不可用。";
                        }
                        else
                        {
                            int coins = WasherConsumeBll.Instance.GetValidCoins(consume.KeyId);
                            Department dept = DepartmentBll.Instance.Get(wxconsume.DepartmentId);
                            
                            //如果其卡内还有超过5元的洗车比，则提示其可用直接启动机器
                            if (coins >= 500)
                            {
                                message.Content = string.Format(
@"卡内余额洗车，请点<a href='http://xc.senlanjidian.com/PublicPlatform/Web/Authorize.aspx?next=PayWash.aspx&appid={0}&board={1}&card=true'>这里</a>。

微信支付洗车，请点<a href='http://xc.senlanjidian.com/PublicPlatform/Web/Authorize.aspx?next=PayWash.aspx&appid={0}&board={1}'>这里</a>。", dept.KeyId, device.BoardNumber);
                            }
                            else
                            {
                                message.Content = string.Format(@"微信支付洗车，请点<a href='http://xc.senlanjidian.com/PublicPlatform/Web/Authorize.aspx?next=PayWash.aspx&appid={0}&board={1}'>这里</a>。", dept.KeyId, device.BoardNumber);
                            }
                        }
                    }
                }
                else if (senceId.StartsWith("7"))
                {
                    //什么也不做
                }
                else
                {
                    message.Content = "参数错误！";
                }
            }

            return message;
        }

        public override IResponseMessageBase OnEvent_ViewRequest(RequestMessageEvent_View requestMessage)
        {
            //说明：这条消息只作为接收，下面的responseMessage到达不了客户端，类似OnEvent_UnsubscribeRequest
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您点击了view按钮，将打开网页：" + requestMessage.EventKey;
            return responseMessage;
        }

        public override IResponseMessageBase OnEvent_MassSendJobFinishRequest(RequestMessageEvent_MassSendJobFinish requestMessage)
        {
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "接收到了群发完成的信息。";
            return responseMessage;
        }

        /// <summary>
        /// 订阅（关注）事件
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_SubscribeRequest(RequestMessageEvent_Subscribe requestMessage)
        {
            var responseMessage = ResponseMessageBase.CreateFromRequestMessage<ResponseMessageText>(requestMessage);

            Department dept = DepartmentBll.Instance.Get(deptId);
            int refererId = -1;
            WasherWeChatConsumeModel wxconsume;

            if (!string.IsNullOrEmpty(requestMessage.EventKey))
            {
                string senceId = requestMessage.EventKey;
                if (senceId.StartsWith("qrscene"))
                {
                    senceId = senceId.Substring(8);
                }

                if (senceId.StartsWith("7"))//推广用户编号以7开头
                {
                    int keyid = Convert.ToInt32(senceId.Substring(1));
                    wxconsume = WasherWeChatConsumeBll.Instance.Get(keyid);
                    if (wxconsume != null)
                    {
                        refererId = wxconsume.KeyId;
                    }
                }
            }

            //判断数据库中是否有这个消费者的记录
            wxconsume = WasherWeChatConsumeBll.Instance.Get(deptId, WeixinOpenId);
            if (wxconsume == null)
            {
                #region 这是不存在的消费者，将其加入到消费者表中
                wxconsume = new WasherWeChatConsumeModel();
                wxconsume.OpenId = WeixinOpenId;
                wxconsume.DepartmentId = deptId;
                wxconsume.RefererId = refererId;
                wxconsume.Memo = "";

                wxconsume.KeyId = WasherWeChatConsumeBll.Instance.Add(wxconsume);
                #endregion

                responseMessage.Content = string.Format("欢迎使用 {0} 洗车机。\r\n请先在“个人中心-我的账户”中完成个人信息绑定，享受会员权益。", dept.Brand);

                AsyncHandleOtherThings(wxconsume.KeyId);
            }
            else
            {
                responseMessage.Content = "欢迎再次回来。";
            }

            return responseMessage;
        }

        private void AsyncHandleOtherThings(int wxid)
        {
            new Thread(() => {
                Thread.Sleep(2000);

                WasherWeChatConsumeModel wxconsume = WasherWeChatConsumeBll.Instance.Get(wxid);
                Department dept = DepartmentBll.Instance.Get(wxconsume.DepartmentId);

                #region 获取微信用户详细信息
                WeixinUserInfoResult result = CommonApi.GetUserInfo(dept.Appid, wxconsume.OpenId);
                if (result.errcode == Senparc.Weixin.ReturnCode.请求成功)
                {
                    wxconsume.NickName = result.nickname;
                    wxconsume.Gender = result.sex == 1 ? "男" : result.sex == 2 ? "女" : "未知";
                    wxconsume.Country = result.country;
                    wxconsume.Province = result.province;
                    wxconsume.City = result.city;
                    wxconsume.UnionId = result.unionid;

                    WasherWeChatConsumeBll.Instance.Update(wxconsume);
                }
                #endregion

                //获取相关设置信息
                WasherDepartmentSetting setting = WasherDepartmentSetting.Instance;
                setting = JsonConvert.DeserializeObject<WasherDepartmentSetting>(dept.Setting);

                string newopenid = wxconsume.OpenId;
                int refererid = wxconsume.RefererId;
                WasherConsumeModel consume;
                int index = -1;
                while ((++index < setting.Point.Referers.Level.Count()) && refererid != -1)
                {
                    if ((consume = WasherConsumeBll.Instance.GetByBinderId(refererid)) != null)
                    {
                        WasherRewardModel reward = new WasherRewardModel();
                        reward.ConsumeId = consume.KeyId;
                        reward.Kind = "新用户关注公众号，老用户送积分。";
                        reward.Memo = string.Format("openid:{0}", newopenid);
                        reward.Points = setting.Point.Referers.Level[index];
                        reward.Time = DateTime.Now;
                        reward.Used = 0;
                        reward.Expired = false;
                        WasherRewardBll.Instance.Add(reward);
                    }

                    wxconsume = WasherWeChatConsumeBll.Instance.Get(refererid);
                    if (wxconsume != null)
                    {
                        refererid = wxconsume.RefererId;
                    }
                    else
                    {
                        break;
                    }
                }
            }).Start();
        }

        /// <summary>
        /// 退订
        /// 实际上用户无法收到非订阅账号的消息，所以这里可以随便写。
        /// unsubscribe事件的意义在于及时删除网站应用中已经记录的OpenID绑定，消除冗余数据。并且关注用户流失的情况。
        /// </summary>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_UnsubscribeRequest(RequestMessageEvent_Unsubscribe requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "有空再来";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件(scancode_push)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodePushRequest(RequestMessageEvent_Scancode_Push requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件";
            return responseMessage;
        }

        /// <summary>
        /// 事件之扫码推事件且弹出“消息接收中”提示框(scancode_waitmsg)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_ScancodeWaitmsgRequest(RequestMessageEvent_Scancode_Waitmsg requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之扫码推事件且弹出“消息接收中”提示框";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出拍照或者相册发图（pic_photo_or_album）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicPhotoOrAlbumRequest(RequestMessageEvent_Pic_Photo_Or_Album requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出拍照或者相册发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出系统拍照发图(pic_sysphoto)
        /// 实际测试时发现微信并没有推送RequestMessageEvent_Pic_Sysphoto消息，只能接收到用户在微信中发送的图片消息。
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicSysphotoRequest(RequestMessageEvent_Pic_Sysphoto requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出系统拍照发图";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出微信相册发图器(pic_weixin)
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_PicWeixinRequest(RequestMessageEvent_Pic_Weixin requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出微信相册发图器";
            return responseMessage;
        }

        /// <summary>
        /// 事件之弹出地理位置选择器（location_select）
        /// </summary>
        /// <param name="requestMessage"></param>
        /// <returns></returns>
        public override IResponseMessageBase OnEvent_LocationSelectRequest(RequestMessageEvent_Location_Select requestMessage)
        {
            var responseMessage = base.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "事件之弹出地理位置选择器";
            return responseMessage;
        }
    }
}