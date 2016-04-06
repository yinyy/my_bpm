using Newtonsoft.Json;
using WeChat.Model;

namespace WeChat.Utils
{
    public class SendMessageHelper
    {
        private static string SerializerObject(object o)
        {
            string json = JsonConvert.SerializeObject(o);
            return json;
        }

        private static void Send(string json)
        {
            WeChatToolkit.PostMessage(json);
        }

        /// <summary>
        /// 发送文本消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="content"></param>
        public static void SendTextMessage(string to, string content)
        {
            var o = new { touser = to, msgtype = MessageType.Text, text = new { content = content } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送图片消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="media"></param>
        public static void SendPictureMessage(string to, string media)
        {
            var o = new { touser = to, msgtype = MessageType.Image, image = new { media_id = media } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送语音消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="media"></param>
        public static void SendVoiceMessage(string to, string media)
        {
            var o = new { touser = to, msgtype = MessageType.Voice, voice = new { media_id = media } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送视频消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="media"></param>
        /// <param name="thumb"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        public static void SendVideoMessage(string to, string media, string thumb, string title, string description)
        {
            var o = new { touser = to, msgtype = MessageType.Video, video = new { media_id = media, thumb_media_id = thumb, title = title, description = description } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送音乐消息
        /// </summary>
        /// <param name="to"></param>
        /// <param name="title"></param>
        /// <param name="description"></param>
        /// <param name="url"></param>
        /// <param name="hqurl"></param>
        /// <param name="thumb"></param>
        public static void SendMusicMessage(string to, string title, string description, string url, string hqurl, string thumb)
        {
            var o = new { touser = to, msgtype = MessageType.Music, music = new { title = title, description = description, musicurl = url, hqmusicurl = hqurl, thumb_media_id = thumb } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送图文消息，点击跳转到外链
        /// </summary>
        /// <param name="to">用户的openid</param>
        /// <param name="articles">{title="title", description="description", url="url", picurl="picurl"}</param>
        public static void SendNewsMessage(string to, params object[] articles)
        {
            var o = new { touser = to, msgtype = MessageType.News, news = new { articles = articles } };
            Send(SerializerObject(o));
        }

        /// <summary>
        /// 发送图文消息，点击跳转到图文消息页面
        /// </summary>
        /// <param name="to"></param>
        /// <param name="media"></param>
        public static void SendNewsMessage(string to, string media)
        {
            var o = new { touser = to, msgtype = MessageType.MpNews, mpnews = new {media_id=media } };
            Send(SerializerObject(o));
        }
    }
}
