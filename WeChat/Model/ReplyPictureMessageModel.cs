using System.Xml.Serialization;
using System.Xml;

namespace WeChat.Model
{
    [XmlRoot("xml")]
    public class ReplyPictureMessageModel:ReplyMessageModel
    {
        public ReplyPictureMessageModel()
        {
            Type = MessageType.Image;
        }

        [XmlElement("Image")]
        public MessagePicture Image { get; set; }
    }

    public class MessagePicture
    {
        [XmlIgnore]
        public string MediaId { get; set; }

        [XmlElement("MediaId")]
        public XmlCDataSection CDataMediaId {
            get
            {
                return new XmlDocument().CreateCDataSection(MediaId);
            }
            set
            {
                MediaId = value.Value;
            }
        }
    }
}
