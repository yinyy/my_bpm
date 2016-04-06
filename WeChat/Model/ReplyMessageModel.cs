using System;
using System.Xml;
using System.Xml.Serialization;

namespace WeChat.Model
{
    public partial class ReplyMessageModel
    {
        protected XmlDocument document;

        public ReplyMessageModel()
        {
            document = new XmlDocument();

            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            Created = (int)(DateTime.Now - startTime).TotalSeconds;
        }

        [XmlIgnore]
        public string To { get; set; }

        [XmlIgnore]
        public string From { get; set; }

        [XmlElement("CreateTime")]
        public int Created { get; set; }

        [XmlIgnore]
        public string Type { get; set; }
    }

    [XmlRoot("xml")]
    public partial class ReplyMessageModel
    {
        [XmlElement("ToUserName")]
        public XmlCDataSection CDataTo
        {
            get
            {
                return document.CreateCDataSection(To);
            }
            set
            {
                To = value.Value;
            }
        }

        [XmlElement("FromUserName")]
        public XmlCDataSection CDataFrom
        {
            get
            {
                return document.CreateCDataSection(From);
            }
            set
            {
                From = value.Value;
            }
        }

        [XmlElement("MsgType")]
        public XmlCDataSection CDataType
        {
            get
            {
                return document.CreateCDataSection(Type);
            }
            set
            {
                Type = value.Value;
            }
        }
    }
}
