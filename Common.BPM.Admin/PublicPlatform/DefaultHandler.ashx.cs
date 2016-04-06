using System;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using Washer.Bll;
using Washer.Model;
using Washer.Processor;
using WeChat.Model;
using WeChat.Utils;

namespace BPM.Admin.PublicPlatform
{
    /// <summary>
    /// DefaultHandler 的摘要说明
    /// </summary>
    public class DefaultHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod.ToUpper() == "GET")
            {
                context.Response.ContentType = "text/plain";

                string signature = context.Request["signature"];
                string timestamp = context.Request["timestamp"];
                string nonce = context.Request["nonce"];
                string echostr = context.Request["echostr"];
                
                if (WeChatToolkit.Validate(signature, timestamp, nonce))
                {
                    context.Response.Write(echostr);
                }
            }
            else if (context.Request.HttpMethod.ToUpper() == "POST")
            {
                ReceivedMessageModel message = null;
                XmlSerializer serializer = new XmlSerializer(typeof(ReceivedMessageModel));
                using (StreamReader reader = new StreamReader(context.Request.InputStream))
                {
                    string xml = reader.ReadToEnd();
                    using (StringReader reader2 = new StringReader(xml))
                    {
                        message = serializer.Deserialize(reader2) as ReceivedMessageModel;
                    }

                    if (message != null)
                    {
                        #region 将消息写入数据库
                        WasherReceivedMessageBll bll = new WasherReceivedMessageBll();
                        bll.Add(new WasherReceivedMessageModel() { OpenId = message.From, Primitive = xml, Type = message.Type, UnixTime = message.Created, Time = DateTime.Now });
                        #endregion
                    }
                }              
                new WasherProcessor().process(message);
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