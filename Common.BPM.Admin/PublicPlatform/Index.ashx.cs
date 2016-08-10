using BPM.Admin.PublicPlatform.MessageHandler;
using BPM.Core.Bll;
using BPM.Core.Model;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.CommonAPIs;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.IO;
using System.Web;

namespace BPM.Admin.PublicPlatform
{
    /// <summary>
    /// DefaultHandler 的摘要说明
    /// </summary>
    public class Index : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            string echostr = context.Request["echostr"];
            string tag = context.Request["tag"];

            int deptId = Convert.ToInt32(tag);
            string token = null;
            Department dept = DepartmentBll.Instance.Get(deptId);
            if (dept != null)
            {
                token = dept.Token;

                if (!AccessTokenContainer.CheckRegistered(dept.Appid))
                {
                    AccessTokenContainer.Register(dept.Appid, dept.Secret);
                }
            }

            //using (StreamWriter writer = new StreamWriter(context.Server.MapPath("~/App_Data/00" + DateTime.Now.Ticks + ".txt")))
            //{
            //    writer.WriteLine(string.Format("signature={0}&timestamp={1}&nonce={2}&echostr={3}&token={5}", signature, timestamp, nonce, echostr, tag, token));
            //}
            
            if (context.Request.HttpMethod.ToUpper() == "GET")
            {
                if (CheckSignature.Check(signature, timestamp, nonce, token))
                {
                    context.Response.Write(echostr);

                    #region 创建用户分组
                    #endregion
                }
                else
                {
                    context.Response.Write("failed!");
                }

                context.Response.End();
            }
            else if (context.Request.HttpMethod.ToUpper() == "POST")
            {
                if (!CheckSignature.Check(signature, timestamp, nonce, token))
                {
                    context.Response.Write("failed!");
                    context.Response.End();
                    return;
                }

                PostModel postModel = new PostModel()
                {
                    Signature = signature,
                    Msg_Signature = context.Request.QueryString["msg_signature"],
                    Timestamp = timestamp,
                    Nonce = nonce,

                    #region  以下保密信息不会（不应该）在网络上传播，请注意 
                    Token = token,
                    EncodingAESKey = dept.Aeskey,
                    AppId = dept.Appid
                    #endregion
                };
                
                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制 
                var maxRecordCount = 10;
                
                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。 
                var messageHandler = new CustomMessageHandler(deptId, context.Request.InputStream, postModel, maxRecordCount);

                try
                {
                    //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。 
                    messageHandler.RequestDocument.Save(
                        context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" +
                                       messageHandler.RequestMessage.FromUserName + ".txt"));

                    //执行微信处理过程 
                    messageHandler.Execute();

                    //测试时可开启，帮助跟踪数据 
                    messageHandler.ResponseDocument.Save(
                         context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" +
                                        messageHandler.ResponseMessage.ToUserName + ".txt"));

                    context.Response.Write(messageHandler.ResponseDocument.ToString());
                    return;
                }
                catch (Exception ex)
                {
                    using (TextWriter tw = new StreamWriter(context.Server.MapPath("~/App_Data/Error_" + DateTime.Now.Ticks + ".txt")))
                    {
                        tw.WriteLine(ex.Message);
                        tw.WriteLine(ex.InnerException.Message);
                        if (messageHandler.ResponseDocument != null)
                        {
                            tw.WriteLine(messageHandler.ResponseDocument.ToString());
                        }
                        tw.Flush();
                        tw.Close();
                    }
                }
                finally
                {
                    context.Response.End();
                }
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