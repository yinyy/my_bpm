using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace WeChat.Model
{
    [XmlRoot("xml")]
    public class ReceivedMessageModel
    {
        [XmlElement("ToUserName")]
        public string To { get; set; }

        [XmlElement("FromUserName")]
        public string From { get; set; }

        [XmlElement("CreateTime")]
        public int Created { get; set; }

        [XmlElement("MsgType")]
        public string Type { get; set; }

        /// <summary>
        /// 文本消息
        /// </summary>
        [XmlElement("Content")]
        public string Content { get; set; }

        [XmlElement("MsgId")]
        public long Id { get; set; }

        /// <summary>
        /// 图片消息
        /// </summary>
        [XmlElement("PicUrl")]
        public string Picture { get; set; }

        /// <summary>
        /// 图片消息、语音消息、视频消息、小视频消息
        /// </summary>
        [XmlElement("MediaId")]
        public string Media { get; set; }

        /// <summary>
        /// 语音消息
        /// </summary>
        [XmlElement("Format")]
        public string Format { get; set; }

        /// <summary>
        /// 开通语音识别
        /// </summary>
        [XmlElement("Recognition")]
        public string Recognition { get; set; }

        /// <summary>
        /// 视频消息、小视频消息
        /// </summary>
        [XmlElement("ThumbMediaId")]
        public string Thumb { get; set; }

        /// <summary>
        /// 地理位置消息
        /// </summary>
        [XmlElement("Location_X")]
        public double X { get; set; }

        /// <summary>
        /// 地理位置消息
        /// </summary>
        [XmlElement("Location_Y")]
        public double Y { get; set; }

        /// <summary>
        /// 地理位置消息
        /// </summary>
        [XmlElement("Scale")]
        public int Scale { get; set; }

        /// <summary>
        /// 地理位置消息
        /// </summary>
        [XmlElement("Label")]
        public string Label { get; set; }

        /// <summary>
        /// 链接消息
        /// </summary>
        [XmlElement("Title")]
        public string Title { get; set; }

        /// <summary>
        /// 链接消息
        /// </summary>
        [XmlElement("Description")]
        public string Description { get; set; }

        /// <summary>
        /// 链接消息
        /// </summary>
        [XmlElement("Url")]
        public string Url { get; set; }

        /// <summary>
        /// 关注事件、取消关注事件、扫码事件、上报地理位置事件
        /// </summary>
        [XmlElement("Event")]
        public string Event { get; set; }

        /// <summary>
        /// 扫码带参数二维码事件
        /// </summary>
        [XmlElement("EventKey")]
        public string Key { get; set; }

        /// <summary>
        /// 扫码带参数二维码事件
        /// </summary>
        [XmlElement("Ticket")]
        public string Ticket { get; set; }

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        [XmlElement("Latitude")]
        public double Latitude { get; set; }

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        [XmlElement("Longitude")]
        public double Longitude { get; set; }

        /// <summary>
        /// 上报地理位置事件
        /// </summary>
        [XmlElement("Precision")]
        public double Precision { get; set; }

        /// <summary>
        /// 电机菜单跳转链接事件
        /// </summary>
        [XmlElement("MenuId")]
        public string Menu { get; set; }

        /// <summary>
        /// 扫码推事件
        /// </summary>
        [XmlElement("ScanCodeInfo")]
        public ScanCode ScanCodeInfo { get; set; }

        [XmlElement("SendPicsInfo")]
        public SendPictures SendPicsInfo { get; set; }

        [XmlElement("SendLocationInfo")]
        public SendLocation SendLocationInfo { get; set; }
    }

    public class ScanCode
    {
        [XmlElement("ScanType")]
        public String Type { get; set; }

        [XmlElement("ScanResult")]
        public string Result { get; set; }
    }

    public class SendPictures
    {
        [XmlElement("Count")]
        public int Count { get; set; }

        [XmlElement("PicList")]
        public PictureItem[] Items { get; set; }
    }

    public class PictureItem
    {
        [XmlElement("PicMd5Sum")]
        public string PictureSum { get; set; }
    }

    public class SendLocation
    {
        [XmlElement("Location_X")]
        public string X { get; set; }

        [XmlElement("Location_Y")]
        public string Y { get; set; }

        [XmlElement("Scale")]
        public string Scale { get; set; }

        [XmlElement("Label")]
        public string Label { get; set; }

        [XmlElement("Poiname")]
        public string Poiname { get; set; }
    }
}
