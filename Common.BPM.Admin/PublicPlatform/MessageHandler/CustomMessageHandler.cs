using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Senparc.Weixin.MP.Entities;
using System.IO;
using Senparc.Weixin.MP.Entities.Request;

namespace BPM.Admin.PublicPlatform.MessageHandler
{
    public partial class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;
            
            //在指定条件下，不使用消息去重
            base.OmitRepeatedMessageFunc = requestMessage =>
            {
                var textRequestMessage = requestMessage as RequestMessageText;
                if (textRequestMessage != null && textRequestMessage.Content == "容错")
                {
                    return false;
                }
                return true;
            };
        }

        public override void OnExecuting()
        {
            //测试MessageContext.StorageData
            if (CurrentMessageContext.StorageData == null)
            {
                CurrentMessageContext.StorageData = 0;
            }
            base.OnExecuting();
        }

        public override void OnExecuted()
        {
            base.OnExecuted();
            CurrentMessageContext.StorageData = ((int)CurrentMessageContext.StorageData) + 1;
        }


        public override IResponseMessageBase OnTextRequest(RequestMessageText requestMessage)
        {
            if (requestMessage.Content == "设备")
            {
                ResponseMessageNews responseMessage = CreateResponseMessage<ResponseMessageNews>();
                Article a = new Article();
                a.Description = "查看已经关注的设备或取消设备关注。";
                a.Title = "设备信息";
                a.Url = "http://agriculture.dyzyxyydwlwsys.cc/PublicPlatform/Web/Device.aspx?oid=" + WeixinOpenId;
                a.PicUrl = "http://agriculture.dyzyxyydwlwsys.cc/PublicPlatform/Web/images/iot.png";

                responseMessage.Articles.Add(a);
                
                return responseMessage;
            }
            else
            {
                var responseMessage = CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "输入了不能识别的命令。";
                return responseMessage;
            }
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //ResponseMessageText也可以是News等其他类型
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
    }
}