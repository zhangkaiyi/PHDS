using System;
using System.Collections.Generic;
using System.Web;
using System.Xml;
using System.Xml.Serialization;

namespace PHDS.Weixin.QY
{
    public interface IRequestMessage
    {
        string ToUserName { get; set; }
        string FromUserName { get; set; }
        string CreateTime { get; set; }
        string MsgType { get; set; }
        string Content { get; set; }
        string PicUrl { get; set; }
        string MediaId { get; set; }
        string ThumbMediaId { get; set; }
        string Location_X { get; set; }
        string Location_Y { get; set; }
        string Precision { get; set; }
        string Scale { get; set; }
        string Label { get; set; }
        string MsgId { get; set; }
        string Event { get; set; }
        string EventKey { get; set; }
        string ScanCodeInfo { get; set; }
        string Title { get; set; }
        string Description { get; set; }
        string Url { get; set; }
        int AgentID { get; set; }
    }

    [XmlRoot("xml")]
    public class RequestMessage : IRequestMessage
    {
        public string ToUserName { get; set; }
        public string FromUserName { get; set; }
        public string CreateTime { get; set; }
        public string MsgType { get; set; }
        public string Content { get; set; }
        public string PicUrl { get; set; }
        public string MediaId { get; set; }
        public string ThumbMediaId { get; set; }
        public string Location_X { get; set; }
        public string Location_Y { get; set; }
        public string Precision { get; set; }
        public string Scale { get; set; }
        public string Label { get; set; }
        public string MsgId { get; set; }
        public string Event { get; set; }
        public string EventKey { get; set; }
        public string ScanCodeInfo { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Url { get; set; }
        public int AgentID { get; set; }
    }
}