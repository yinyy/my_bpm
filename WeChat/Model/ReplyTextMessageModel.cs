using System.Xml;
using System.Xml.Serialization;

namespace WeChat.Model
{
    public partial class ReplyTextMessageModel : ReplyMessageModel
    {
        public ReplyTextMessageModel()
        {
            Type = MessageType.Text;
        }

        [XmlIgnore]
        public string Content { get; set; }
    }

    [XmlRoot("xml")]
    public partial class ReplyTextMessageModel : ReplyMessageModel
    {
        [XmlElement("Content")]
        public XmlCDataSection CDataContent {
            get
            {
                return document.CreateCDataSection(Content);
            }
            set
            {
                Content = value.Value;
            }
        }
    }
}
