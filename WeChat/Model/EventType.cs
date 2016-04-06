using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Model
{
    public class EventType
    {
        /// <summary>
        /// 关注事件
        /// </summary>
        public const string Subscribe = "subscribe";

        /// <summary>
        /// 取消关注事件
        /// </summary>
        public const string Unsubscribe = "unsubscribe";

        public const string Scan = "SCAN";

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        public const string Location = "LOCATION";

        /// <summary>
        /// 点击菜单拉消息事件
        /// </summary>
        public const string Click = "CLICK";

        /// <summary>
        /// 点击菜单跳转链接事件
        /// </summary>
        public const string View = "VIEW";

        /// <summary>
        /// 扫码推事件
        /// </summary>
        public const string ScanCodePush = "scancode_push";

        /// <summary>
        /// 扫码推事件且弹出消息接收中
        /// </summary>
        public const string ScanCodeWaitMsg = "scancode_waitmsg";

        /// <summary>
        /// 弹出系统拍照发图的事件推送
        /// </summary>
        public const string Photo = "pic_sysphoto";

        /// <summary>
        /// 弹出拍照或者相册发图的事件推送
        /// </summary>
        public const string PhotoOrAlbum = "pic_photo_or_album";

        /// <summary>
        /// 弹出微信相册发图器的事件推送
        /// </summary>
        public const string Weixin = "pic_weixin";

        /// <summary>
        /// 弹出地理位置选择器的事件推送
        /// </summary>
        public const string LocationSelect = "location_select";
    }
}
