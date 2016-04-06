using Newtonsoft.Json;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using WeChat.Model;

namespace WeChat.Utils
{
    public class WeChatToolkit
    {
        private static string _AccessToken = null;
        private static DateTime _AccessTokenExpires = DateTime.Now;
        private static WeChatAppConfiguration _AppConf = null;
        
        private class AccessTokenClass
        {
            public string access_token = null;
            public int expires_in = 0;
        }

        public static WeChatAppConfiguration AppConfiguration
        {
            get
            {
                if (_AppConf == null)
                {
                    _AppConf = new WeChatAppConfiguration();
                    _AppConf._AppId = ConfigurationManager.AppSettings["appid"];
                    _AppConf._EncodingAESKey = ConfigurationManager.AppSettings["aeskey"];
                    _AppConf._AppSecret = ConfigurationManager.AppSettings["secret"];
                    _AppConf._Token = ConfigurationManager.AppSettings["token"];
                }

                return _AppConf;
            }
        }

        public static string AccessToken
        {
            get
            {
                //如果当前的access_token为null或者当前的时间与上次获取access_token相比，超过了7100秒，则刷新access_token
                if (_AccessToken == null || _AccessTokenExpires.Subtract(DateTime.Now).Seconds <= 10)
                {
                    string url = string.Format("https://api.weixin.qq.com/cgi-bin/token?grant_type=client_credential&appid={0}&secret={1}", AppConfiguration.AppId, AppConfiguration.AppSecret);

                    WebRequest request = WebRequest.Create(url);
                    request.Method = "GET";
                    using (WebResponse response = request.GetResponse())
                    {
                        using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                        {
                            var o = new { id = "", name = "" };
                            var p = JsonConvert.DeserializeObject("");
                            string data = reader.ReadToEnd();
                            if (data.IndexOf("errcode") == -1)
                            {
                                AccessTokenClass atc = JsonConvert.DeserializeObject<AccessTokenClass>(data);
                                _AccessToken = atc.access_token;
                                _AccessTokenExpires = DateTime.Now.AddSeconds(atc.expires_in);
                            }
                        }
                    }
                }

                return _AccessToken;
            }
        }
        
        public static int GetCurrentTimestamp()
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));

            return (int)(DateTime.Now - startTime).TotalSeconds;
        }

        /// <summary>
        /// 验证token、timestamp、nonce的SHA1验证是否与signature相同
        /// </summary>
        /// <param name="signature"></param>
        /// <param name="timestamp"></param>
        /// <param name="nonce"></param>
        /// <returns>true：通过验证，false：未通过验证</returns>
        public static bool InterfaceConfigurationValidate(string signature, string timestamp, string nonce, string token)
        {
            string[] arr = { token, timestamp, nonce };
            Array.Sort(arr);
            string str = string.Join("", arr);

            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();
            byte[] bs = Encoding.Default.GetBytes(str);
            bs = sha1.ComputeHash(bs);

            StringBuilder sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.AppendFormat("{0:x2}", b);
            }

            str = sb.ToString();

            return str == signature;
        }

        ///// <summary>
        ///// XML反序列化为object
        ///// </summary>
        ///// <param name="xml"></param>
        ///// <param name="type"></param>
        ///// <returns></returns>
        //public static object DeserializeObject(string xml, Type type)
        //{
        //    object obj = null;

        //    XmlSerializer serializer = new XmlSerializer(type);
        //    using (MemoryStream stream = new MemoryStream(Encoding.Default.GetBytes(xml)))
        //    {
        //        obj = serializer.Deserialize(stream);
        //    }

        //    return obj;
        //}

        /// <summary>
        /// 对象序列化为XML
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToXmlString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.Encoding = Encoding.UTF8;
            settings.Indent = false;
            settings.OmitXmlDeclaration = true;
            
            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add(string.Empty, string.Empty);

            string xml = "";

            using(StringWriter sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw, settings))
                {
                    serializer.Serialize(writer, obj, ns);
                }

                xml = sw.ToString().Trim();
            }

            return xml;
        }

        /// <summary>
        /// 回复用户消息，格式为XML
        /// </summary>
        /// <param name="message"></param>
        public static void ReplyMessage(object obj = null)
        {
            string message = "success";

            if (obj is ReplyMessageModel)
            {
                //WeChatMenuEventModel model = obj as WeChatMenuEventModel;
                //model.CreateTime = GetCurrentTimestamp();
                message = WeChatToolkit.ToXmlString(obj);
            }
            
            HttpContext.Current.Response.Write(message);
            HttpContext.Current.Response.Flush();

            HttpContext.Current.Response.Close();
        }

        /// <summary>
        /// 向用户发送消息，格式为JSON
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="callback"></param>
        //public static void PostMessage(string message, Action<String> callback = null)
        public static string PostMessage(string message)
        {
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/message/custom/send?access_token={0}", AccessToken);

            byte[] bs = Encoding.UTF8.GetBytes(message);

            WebRequest request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = bs.Length;

            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(bs, 0, bs.Length);
                writer.Flush();
            }

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        //public static void SendCommand(string url, string message=null, Action<String> callback = null)
        public static string SendCommand(string url, string message = null)
        {
            WebRequest request = WebRequest.Create(url);

            if (message != null)
            {
                byte[] bs = Encoding.UTF8.GetBytes(message);

                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = bs.Length;

                using (Stream writer = request.GetRequestStream())
                {
                    writer.Write(bs, 0, bs.Length);
                    writer.Flush();
                }
            }
            else
            {
                request.Method = "GET";
            }

            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }

        public static BasicInfoModel GetBasicInfo(string openId)
        {
            BasicInfoModel basicInfo = null;
            string url = string.Format("https://api.weixin.qq.com/cgi-bin/user/info?access_token={0}&openid={1}&lang=zh_CN", AccessToken, openId);

            WebRequest request = WebRequest.Create(url);
            request.Method = "GET";
            using (StreamReader reader = new StreamReader(request.GetResponse().GetResponseStream()))
            {
                string msg = reader.ReadToEnd();

                basicInfo = JsonConvert.DeserializeObject<BasicInfoModel>(msg);
            }

            return basicInfo;
        }
    }
}
