using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PHDS.Weixin.QY.Enums
{
    public enum ResponseType
    {
        Text,
        Image,
        Voice,
        Video,
        News
    }
}
namespace PHDS.Weixin.QY
{
    public interface IMessage
    {
        string ToUserName { get; set; }
        string FromUserName { get; set; }
        string CreateTime { get; set; }
        string MsgType { get; set; }
    }
    
    public interface IResponseMessageText : IMessage
    {
        string Content { get; set; }
    }
    public  interface IResponseMessageNews : IMessage
    {

    }

    public interface IResponseMessage
    {
        string ToUserName { get; set; }
        string FromUserName { get; set; }
        string CreateTime { get; set; }
        string MsgType { get; set; }
        string Content { get; set; }
        ResponseMessageInnerType.Media Image { get; set; }
        ResponseMessageInnerType.Media Voice { get; set; }
        string Format { get; set; }
        ResponseMessageInnerType.Video Video { get; set; }
        int ArticleCount { get; set; }
        List<ResponseMessageInnerType.Article> Articles { get; set; }
    }

    [XmlRoot("xml")]
    public class ResponseMessage : IResponseMessage
    {
        public static IResponseMessage CreateFromRequestMessage(IRequestMessage requestMessage, Enums.ResponseType msgType)
        {
            IResponseMessage responseMessage = null;
            switch (msgType)
            {
                case Enums.ResponseType.Text:
                    responseMessage = new ResponseMessage
                    {
                    };
                    break;
                case Enums.ResponseType.News:
                    responseMessage = new ResponseMessage
                    {
                        Articles = new List<ResponseMessageInnerType.Article>()
                    };
                    break;
                case Enums.ResponseType.Image:
                    responseMessage = new ResponseMessage
                    {
                        Image = new ResponseMessageInnerType.Media()
                    };
                    break;
                case Enums.ResponseType.Voice:
                    responseMessage = new ResponseMessage
                    {
                        Voice = new ResponseMessageInnerType.Media()
                    };
                    break;
                case Enums.ResponseType.Video:
                    responseMessage = new ResponseMessage
                    {
                        Video = new ResponseMessageInnerType.Video()
                    };
                    break;
            }
            if (responseMessage != null)
            {
                responseMessage.MsgType = msgType.ToString().ToLower();
                responseMessage.ToUserName = requestMessage.FromUserName;
                responseMessage.FromUserName = requestMessage.ToUserName;
                responseMessage.CreateTime = Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString(); //使用当前最新时间
            }
            return responseMessage;
        }
        [XmlIgnore]
        public string ToUserName { get; set; }   //CDATA
        [XmlElement("ToUserName")]
        public XmlNode CDATA_ToUserName
        {
            get
            {
                if (string.IsNullOrEmpty(ToUserName))
                {
                    return null;
                }
                XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                node.InnerText = ToUserName;
                return node;
            }
            set { ToUserName = value.InnerText; }
        }
        [XmlIgnore]
        public string FromUserName { get; set; } //CDATA
        [XmlElement("FromUserName")]
        public XmlNode CDATA_FromUserName
        {
            get
            {
                if (string.IsNullOrEmpty(FromUserName))
                {
                    return null;
                }
                XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                node.InnerText = FromUserName;
                return node;
            }
            set { FromUserName = value.InnerText; }
        }
        public string CreateTime { get; set; }
        [XmlIgnore]
        public string MsgType { get; set; }     //CDATA
        [XmlElement("MsgType")]
        public XmlNode CDATA_MsgType
        {
            get
            {
                if (string.IsNullOrEmpty(MsgType))
                {
                    return null;
                }
                XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                node.InnerText = MsgType;
                return node;
            }
            set { MsgType = value.InnerText; }
        }
        [XmlIgnore]
        public string Content { get; set; }      //CDATA
        [XmlElement("Content")]
        public XmlNode CDATA_Content
        {
            get
            {
                if (string.IsNullOrEmpty(Content))
                {
                    return null;
                }
                XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                node.InnerText = Content;
                return node;
            }
            set { Content = value.InnerText; }
        }
        public ResponseMessageInnerType.Media Image { get; set; }
        public ResponseMessageInnerType.Media Voice { get; set; }
        [XmlIgnore]
        public string Format { get; set; }      //CDATA
        [XmlElement("Format")]
        public XmlNode CDATA_Format
        {
            get
            {
                if (string.IsNullOrEmpty(Format))
                {
                    return null;
                }
                XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                node.InnerText = Format;
                return node;
            }
            set { Format = value.InnerText; }
        }
        public ResponseMessageInnerType.Video Video { get; set; }
        public int ArticleCount
        {
            get { return Articles == null ? 0 : Articles.Count; }
            set { }
        }
        [XmlArray("Articles"), XmlArrayItem("item")]
        public List<ResponseMessageInnerType.Article> Articles { get; set; }
    }
    public class ResponseMessageInnerType
    {
        public class Media
        {
            [XmlIgnore]
            public string MediaId { get; set; }
            [XmlElement("MediaId")]
            public XmlNode CDATA_MediaId
            {
                get
                {
                    if (string.IsNullOrEmpty(MediaId))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = MediaId;
                    return node;
                }
                set { MediaId = value.InnerText; }
            }
        }
        public class Video
        {
            [XmlIgnore]
            public string MediaId { get; set; }
            [XmlElement("MediaId")]
            public XmlNode CDATA_MediaId
            {
                get
                {
                    if (string.IsNullOrEmpty(MediaId))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = MediaId;
                    return node;
                }
                set { MediaId = value.InnerText; }
            }
            [XmlIgnore]
            public string Title { get; set; }
            [XmlElement("Title")]
            public XmlNode CDATA_Title
            {
                get
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = Title;
                    return node;
                }
                set { Title = value.InnerText; }
            }
            [XmlIgnore]
            public string Description { get; set; }
            [XmlElement("Description")]
            public XmlNode CDATA_Description
            {
                get
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = Description;
                    return node;
                }
                set { Description = value.InnerText; }
            }
        }
        public class Article
        {
            [XmlIgnore]
            public string Title { get; set; }      //CDATA
            [XmlElement("Title")]
            public XmlNode CDATA_Title
            {
                get
                {
                    if (string.IsNullOrEmpty(Title))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = Title;
                    return node;
                }
                set { Title = value.InnerText; }
            }
            [XmlIgnore]
            public string Description { get; set; } //CDATA
            [XmlElement("Description")]
            public XmlNode CDATA_Description
            {
                get
                {
                    if (string.IsNullOrEmpty(Description))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = Description;
                    return node;
                }
                set { Description = value.InnerText; }
            }
            [XmlIgnore]
            public string PicUrl { get; set; }      //CDATA
            [XmlElement("PicUrl")]
            public XmlNode CDATA_PicUrl
            {
                get
                {
                    if (string.IsNullOrEmpty(PicUrl))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = PicUrl;
                    return node;
                }
                set { PicUrl = value.InnerText; }
            }
            [XmlIgnore]
            public string Url { get; set; }         //CDATA
            [XmlElement("Url")]
            public XmlNode CDATA_Url
            {
                get
                {
                    if (string.IsNullOrEmpty(Url))
                    {
                        return null;
                    }
                    XmlNode node = new XmlDocument().CreateNode(XmlNodeType.CDATA, "", "");
                    node.InnerText = Url;
                    return node;
                }
                set { Url = value.InnerText; }
            }
        }

    }
}