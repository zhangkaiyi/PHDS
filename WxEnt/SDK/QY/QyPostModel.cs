using System;
using System.Collections.Generic;
using System.Web;
using System.Xml.Serialization;

namespace PHDS.Weixin.QY
{
    /// <summary>
    /// 接收解密信息统一接口
    /// </summary>
    public interface IEncryptPostModel
    {
        string Signature { get; set; }
        string Msg_Signature { get; set; }
        string Timestamp { get; set; }
        string Nonce { get; set; }

        //以下信息不会出现在微信发过来的信息中，都是企业号后台需要设置（获取的）的信息，用于扩展传参使用
        string Token { get; set; }
        string EncodingAESKey { get; set; }
    }

    /// <summary>
    /// 接收加密信息统一基类
    /// </summary>
    public class EncryptPostModel : IEncryptPostModel
    {
        public string Signature { get; set; }
        public string Msg_Signature { get; set; }
        public string Timestamp { get; set; }
        public string Nonce { get; set; }

        //以下信息不会出现在微信发过来的信息中，都是企业号后台需要设置（获取的）的信息，用于扩展传参使用
        public string Token { get; set; }
        public string EncodingAESKey { get; set; }
        public string Secret { get; set; }

        /// <summary>
        /// 设置服务器内部保密信息
        /// </summary>
        /// <param name="token"></param>
        /// <param name="encodingAESKey"></param>
        /// <param name="appId"></param>
        public virtual void SetSecretInfo(string token, string encodingAESKey)
        {
            Token = token;
            EncodingAESKey = encodingAESKey;
        }
    }
    /// <summary>
    /// 微信企业号服务器Post过来的参数集合（不包括PostData）
    /// </summary>
    public class QyPostModel : EncryptPostModel
    {
        //以下信息不会出现在微信发过来的信息中，都是企业号后台需要设置（获取的）的信息，用于扩展传参使用
        public string CorpId { get; set; }
    }

    [XmlRoot("xml")]
    public class EncryptPostData
    {
        public string ToUserName { get; set; }
        public string Encrypt { get; set; }
        public int AgentID { get; set; }
    }
}