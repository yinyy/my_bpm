using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Model
{
    public class MessageType
    {
        /// <summary>
        /// 文本消息
        /// </summary>
        public const string Text = "text";

        /// <summary>
        /// 图片消息
        /// </summary>
        public const string Image = "image";

        /// <summary>
        /// 语音消息
        /// </summary>
        public const string Voice = "voice";

        /// <summary>
        /// 视频消息
        /// </summary>
        public const string Video = "video";

        /// <summary>
        /// 小视频消息
        /// </summary>
        public const string ShortVideo = "shortvideo";

        /// <summary>
        /// 地理位置消息
        /// </summary>
        public const string Location = "location";

        /// <summary>
        /// 链接消息
        /// </summary>
        public const string Link = "link";

        /// <summary>
        /// 音乐消息
        /// </summary>
        public const string Music = "music";

        /// <summary>
        /// 图文消息，点击跳转到外链
        /// </summary>
        public const string News = "news";

        /// <summary>
        /// 图文消息，点击跳转到图文消息页面
        /// </summary>
        public const string MpNews = "mpnews";
        
        /// <summary>
        /// 事件
        /// </summary>
        public const string Event = "event";
    }
}
