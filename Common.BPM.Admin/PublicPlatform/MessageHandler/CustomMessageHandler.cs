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
        private string appId;
        //private string appSecret;

        private int deptId;

        public CustomMessageHandler(int deptId, Stream inputStream, PostModel postModel, int maxRecordCount = 0)
            : base(inputStream, postModel, maxRecordCount)
        {
            this.deptId = deptId;

            //这里设置仅用于测试，实际开发可以在外部更全局的地方设置，
            //比如MessageHandler<MessageContext>.GlobalWeixinContext.ExpireMinutes = 3。
            WeixinContext.ExpireMinutes = 3;

            if (!string.IsNullOrEmpty(postModel.AppId))
            {
                appId = postModel.AppId;//通过第三方开放平台发送过来的请求
            }

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

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            //ResponseMessageText也可以是News等其他类型
            var responseMessage = CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "这条消息来自DefaultResponseMessage。";
            return responseMessage;
        }
    }
}