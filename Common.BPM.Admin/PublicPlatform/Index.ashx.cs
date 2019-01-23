using BPM.Admin.PublicPlatform.MessageHandler;
using BPM.Core.Bll;
using BPM.Core.Model;
using Course.Common.Bll;
using Course.Common.Model;
using Newtonsoft.Json;
using Senparc.Weixin;
using Senparc.Weixin.MP;
using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;

namespace BPM.Admin.PublicPlatform
{
    /// <summary>
    /// Index 的摘要说明
    /// </summary>
    public class Index : IHttpHandler, IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            string signature = context.Request["signature"];
            string timestamp = context.Request["timestamp"];
            string nonce = context.Request["nonce"];
            string echostr = context.Request["echostr"];
            string tag = context.Request["tag"];

            ////这里的tag表示的是部门编号
            //int deptId = Convert.ToInt32(tag);
            ////检查部门是否存在
            //CommonSettingModel setting = CommonSettingBll.Instance.Get(deptId);
            //if (setting == null)
            //{
            //    context.Response.Write("参数错误。");
            //    return;
            //}
            

            if (context.Request.HttpMethod == "GET")
            {
                if (CheckSignature.Check(signature, timestamp, nonce, Config.SenparcWeixinSetting.Token))
                {
                    context.Response.Write(echostr); //返回随机字符串则表示验证通过
                }
                else
                {
                    context.Response.Write("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                }
            }
            else if (context.Request.HttpMethod == "POST")
            {
                if (!CheckSignature.Check(signature, timestamp, nonce, Config.SenparcWeixinSetting.Token))
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
                    Token = Config.SenparcWeixinSetting.Token,
                    EncodingAESKey = Config.SenparcWeixinSetting.EncodingAESKey,
                    AppId = Config.SenparcWeixinSetting.WeixinAppId
                    #endregion
                };

                //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制 
                var maxRecordCount = 10;

                //自定义MessageHandler，对微信请求的详细判断操作都在这里面。 
                var messageHandler = new CustomMessageHandler(0, context.Request.InputStream, postModel, maxRecordCount);

                try
                {
                    //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。 
                    //messageHandler.RequestDocument.Save(
                    //    context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Request_" +
                    //                   messageHandler.RequestMessage.FromUserName + ".txt"));

                    //执行微信处理过程 
                    messageHandler.Execute();

                    //测试时可开启，帮助跟踪数据 
                    //messageHandler.ResponseDocument.Save(
                    //     context.Server.MapPath("~/App_Data/" + DateTime.Now.Ticks + "_Response_" +
                    //                    messageHandler.ResponseMessage.ToUserName + ".txt"));

                    if (messageHandler.ResponseDocument != null)
                    {
                        context.Response.Write(messageHandler.ResponseDocument.ToString());
                    }
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