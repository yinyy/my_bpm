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
    public class ReplyTextPictureMessageModel : ReplyMessageModel
    {
        public ReplyTextPictureMessageModel()
        {
            Type = MessageType.News;
        }

        [XmlElement("ArticleCount")]
        public int ArticleCount { get; set; }

        [XmlArray("Articles")]
        public TextPictureItem[] Articles { get; set; }

        public partial class TextPictureItem
        {
            private XmlDocument _document;

            public TextPictureItem()
            {
                _document = new XmlDocument();
            }

            [XmlIgnore]
            public string Title { get; set; }

            [XmlIgnore]
            public string Description { get; set; }

            [XmlIgnore]
            public string PictureUrl { get; set; }

            [XmlIgnore]
            public string Url { get; set; }
        }

        [XmlType(TypeName ="item")]
        public partial class TextPictureItem
        {
            [XmlElement("Title")]
            public XmlCDataSection CDataTitle
            {
                get
                {
                    return _document.CreateCDataSection(Title);
                }
                set
                {
                    Title = value.Value;
                }
            }

            [XmlElement("Description")]
            public XmlCDataSection CDataDescription
            {
                get
                {
                    return _document.CreateCDataSection(Description);
                }
                set
                {
                    Description = value.Value;
                }
            }

            [XmlElement("PicUrl")]
            public XmlCDataSection CDataPictureUrl
            {
                get
                {
                    return _document.CreateCDataSection(PictureUrl);
                }
                set
                {
                    PictureUrl = value.Value;
                }
            }

            [XmlElement("Url")]
            public XmlCDataSection CDataUrl
            {
                get
                {
                    return _document.CreateCDataSection(Url);
                }
                set
                {
                    Url = value.Value;
                }
            }
        }
    }
}
