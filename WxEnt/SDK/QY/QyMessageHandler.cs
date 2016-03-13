using System;
using System.Collections.Generic;
using System.Web;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;
using Tencent;
using Newtonsoft.Json;

namespace PHDS.Weixin.QY
{
    public class QyMessageHandler
    {
        private QyPostModel _postModel;
        public Dictionary<int, RequestAgent> Agents = new Dictionary<int, RequestAgent>();

        public void BindAgentHandler(int agentid, RequestAgent agentobj)
        {
            if (Agents.ContainsKey(agentid))
            {
                return;
            }
            Agents.Add(agentid, agentobj);
        }

        public QyMessageHandler(Stream inputStream, QyPostModel postModel = null)
        {
            //var postDataStr = GetEncryptPostDataString(inputStream);

           RequestDocument = CommonInitialize(inputStream, postModel);
        }

        public string CommonInitialize(Stream inputStream, QyPostModel postModel)
        {
            _postModel = postModel as QyPostModel;

            //1、从Stream获取加密字符串
            var postDataStr = GetEncryptPostDataString(inputStream);
            EncryptPostData = XLH.DeserializeXmlFromString<EncryptPostData>(postDataStr, Encoding.UTF8);

            //2、解密：获得明文字符串
            WXBizMsgCrypt msgCrypt = new WXBizMsgCrypt(_postModel.Token, _postModel.EncodingAESKey, _postModel.CorpId);
            string msgXml = null;
            var result = msgCrypt.DecryptMsg(_postModel.Msg_Signature, _postModel.Timestamp, _postModel.Nonce, postDataStr, ref msgXml);

            //判断result类型
            if (result != 0)
            {
                //验证没有通过，取消执行
                CancelExcute = true;
                return null;
            }

            //XmlDocument requestDocument = new XmlDocument();
            //requestDocument.LoadXml(msgXml);
            //3、对解密后的字符串反序列化
            RequestMessage = XLH.DeserializeXmlFromString<RequestMessage>(msgXml, Encoding.UTF8);

            return msgXml;

        }


        /// <summary>
        /// 根据当前的RequestMessage创建指定类型的ResponseMessage
        /// </summary>
        /// <typeparam name="TR">基于ResponseMessageBase的响应消息类型</typeparam>
        /// <returns></returns>
        public TR CreateResponseMessage<TR>() where TR : ResponseMessage
        {
            if (RequestMessage == null)
            {
                return null;
            }

            //return RequestMessage.CreateResponseMessage<TR>();
            return null;
        }

        /// <summary>
        /// 执行微信请求
        /// </summary>
        public void Execute()
        {
            if (CancelExcute)
            {
                return;
            }

            OnExecuting();

            if (CancelExcute)
            {
                return;
            }

            try
            {
                if (RequestMessage == null)
                {
                    return;
                }
                switch (RequestMessage.MsgType)
                {
                    case "text":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnTextRequest(RequestMessage);
                        break;
                    case "image":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnImageRequest(RequestMessage);
                        break;
                    case "voice":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnVoiceRequest(RequestMessage);
                        break;
                    case "shortvideo":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnShortVoiceRequest(RequestMessage);
                        break;
                    case "video":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnVideoRequest(RequestMessage);
                        break;
                    case "location":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnLocationRequest(RequestMessage);
                        break;
                    case "link":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnLinkRequest(RequestMessage);
                        break;
                    case "event":
                        ResponseMessage = Agents[RequestMessage.AgentID].OnEventRequest(RequestMessage);
                        switch (RequestMessage.Event)
                        {
                            case "click":
                                break;
                            case "subscribe":
                                break;
                            case "unsubscribe":
                                break;
                            case "location":
                                break;
                            case "view":
                                break;
                            case "scancode_push":
                                break;
                            case "scancode_waitmsg":
                                break;
                            case "pic_sysphoto":
                                break;
                            case "pic_photo_or_album":
                                break;
                            case "pic_weixin":
                                break;
                            case "location_select":
                                break;
                            case "enter_agent":
                                break;
                            case "batch_job_result":
                                break;
                        }
                        break;
                    default:
                        break;
                }

                //记录上下文
                //if (WeixinContextGlobal.UseWeixinContext && ResponseMessage != null)
                //{
                //    WeixinContext.InsertMessage(ResponseMessage);
                //}
            }
            catch (Exception )
            {
                //throw new Exception("MessageHandler中Execute()过程发生错误：" + ex.Message, ex);
            }
            finally
            {
                OnExecuted();
            }
        }

        public virtual void OnExecuting()
        {
            Debug.WriteLine("OnExecuting");
        }
        public virtual void OnExecuted()
        {
            Debug.WriteLine("OnExecuted");
        }

        public string GetEncryptPostDataString(Stream inputStream)
        {
            byte[] postbytes = new byte[inputStream.Length];
            inputStream.Read(postbytes, 0, (Int32)inputStream.Length);
            return Encoding.UTF8.GetString(postbytes); ;
        }

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AgentID
        {
            get
            {
                return EncryptPostData != null ? EncryptPostData.AgentID : -1;
            }
        }

        /// <summary>
        /// 原始加密信息
        /// </summary>
        public EncryptPostData EncryptPostData { get; set; }

        /// <summary>
        /// 根据ResponseMessageBase获得转换后的ResponseDocument
        /// 注意：这里每次请求都会根据当前的ResponseMessageBase生成一次，如需重用此数据，建议使用缓存或局部变量
        /// </summary>
        public string ResponseDocument
        {
            get
            {
                if (ResponseMessage == null)
                {
                    return null;
                }
                return XLH.SerializeXmlToString<ResponseMessage>(ResponseMessage as ResponseMessage);
            }
        }
        /// <summary>
        /// 最后返回的ResponseDocument。
        /// 这里是Senparc.Weixin.QY，应当在ResponseDocument基础上进行加密（每次获取重新加密，所以结果会不同）
        /// </summary>
        public string FinalResponseDocument
        {
            get
            {
                if (ResponseDocument == null)
                {
                    return null;
                }

                var timeStamp = DateTime.Now.Ticks.ToString();
                var nonce = DateTime.Now.Ticks.ToString();

                WXBizMsgCrypt msgCrypt = new WXBizMsgCrypt(_postModel.Token, _postModel.EncodingAESKey, _postModel.CorpId);
                string finalResponseXml = null;
                msgCrypt.EncryptMsg(ResponseDocument, timeStamp, nonce, ref finalResponseXml);//TODO:这里官方的方法已经把EncryptResponseMessage对应的XML输出出来了

                return finalResponseXml;
            }
        }

        /// <summary>
        /// 取消执行Execute()方法。一般在OnExecuting()中用于临时阻止执行Execute()。
        /// 默认为False。
        /// 如果在执行OnExecuting()执行前设为True，则所有OnExecuting()、Execute()、OnExecuted()代码都不会被执行。
        /// 如果在执行OnExecuting()执行过程中设为True，则后续Execute()及OnExecuted()代码不会被执行。
        /// 建议在设为True的时候，给ResponseMessage赋值，以返回友好信息。
        /// </summary>
        public bool CancelExcute { get; set; }

        /// <summary>
        /// 在构造函数中转换得到原始XML数据
        /// </summary>
        public string RequestDocument { get; set; }

        //protected Stream InputStream { get; set; }
        /// <summary>
        /// 请求实体
        /// </summary>
        public virtual IRequestMessage RequestMessage { get; set; }
        /// <summary>
        /// 响应实体
        /// 正常情况下只有当执行Execute()方法后才可能有值。
        /// 也可以结合Cancel，提前给ResponseMessage赋值。
        /// </summary>
        public virtual IResponseMessage ResponseMessage { get; set; }

        /// <summary>
        /// 是否使用了MessageAgent代理
        /// </summary>
        public bool UsedMessageAgent { get; set; }

        /// <summary>
        /// 忽略重复发送的同一条消息（通常因为微信服务器没有收到及时的响应）
        /// </summary>
        public bool OmitRepeatedMessage { get; set; }

    }


    public class XLH
    {
        /// <summary>
        /// XML序列化某一类型到指定的文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        public static void SerializeXmlToFile<T>(string filePath, T obj)
        {
            try
            {
                XmlSerializerNamespaces xsNamespaces = new XmlSerializerNamespaces();
                xsNamespaces.Add(string.Empty, string.Empty);
                XmlWriterSettings xwSettings = new XmlWriterSettings();
                xwSettings.OmitXmlDeclaration = true;
                xwSettings.Indent = true;
                XmlWriter xWriter = XmlWriter.Create(filePath, xwSettings);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                xs.Serialize(xWriter, obj, xsNamespaces);
            }
            catch (Exception)
            {
            }
        }
        /// <summary>
        /// XML序列化某一类型到字符串
        /// </summary>
        /// <param name="obj">要进行序列化的类型变量</param>
        /// <param name="type"></param>
        public static String SerializeXmlToString<T>(T obj)
        {
            try
            {
                XmlSerializerNamespaces xsNamespaces = new XmlSerializerNamespaces();
                xsNamespaces.Add(string.Empty, string.Empty);
                XmlWriterSettings xwSettings = new XmlWriterSettings();
                xwSettings.OmitXmlDeclaration = true;
                xwSettings.Indent = true;
                MemoryStream mStream = new MemoryStream();
                XmlWriter xWriter = XmlWriter.Create(mStream, xwSettings);
                System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                xs.Serialize(xWriter, obj, xsNamespaces);
                Byte[] bytes = mStream.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// 从某一XML文件反序列化到某一类型
        /// </summary>
        /// <param name="filePath">待反序列化的XML文件名称</param>
        /// <param name="type">反序列化出的</param>
        /// <returns></returns>
        public static T DeserializeXmlFromFile<T>(string filePath)
        {
            try
            {
                if (!System.IO.File.Exists(filePath))
                    throw new ArgumentNullException(filePath + " not Exists");

                using (System.IO.StreamReader reader = new System.IO.StreamReader(filePath))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }
        /// <summary>
        /// 从XML的String反序列化到某一类型
        /// </summary>
        /// <param name="xmlString">待反序列化的XML的String</param>
        /// <param name="type">反序列化出的</param>
        /// <returns></returns>
        public static T DeserializeXmlFromString<T>(string xmlString, Encoding encode)
        {
            try
            {
                byte[] array = encode.GetBytes(xmlString);
                MemoryStream stream = new MemoryStream(array);             //convert stream 2 string      
                using (System.IO.StreamReader reader = new System.IO.StreamReader(stream))
                {
                    System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    T ret = (T)xs.Deserialize(reader);
                    return ret;
                }
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public static String SerializeJsonToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
        public static T DeserializeJsonFromString<T>(string jsonString)
        {
            return JsonConvert.DeserializeObject<T>(jsonString);
        }
    }
}