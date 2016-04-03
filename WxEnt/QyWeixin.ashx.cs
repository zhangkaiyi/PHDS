using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using PHDS.Weixin.QY;
using PHDS.Weixin;
using PHDS.DBUtility;

namespace QyWeixin
{
    /// <summary>
    /// WxEnt 的摘要说明
    /// </summary>
    public class QyEntry : IHttpHandler
    {
        public static readonly string Token = "PHDS";
        public static readonly string EncodingAESKey = "heUKgrsuTsYexBX9xKSGNt6E1NrcW3JNxjPdbZNMJ6H";
        public static readonly string CorpId = "wx87c90793c5376e09";
        public static readonly string Secret = "NqQ5v-QatUFBfxeQ7ySDw3cawwwh4xAJtvPtGB33qTOKZDq3GiXOtoJQGsMhVrdt";
        public static readonly string sqlConnectstr = "server=122.225.47.230,6019;database=pinhua;uid=sa;pwd=benny0922;connect timeout=5";
        public void ProcessRequest(HttpContext context)
        {
            var msg_signature = context.Request["msg_signature"] ?? string.Empty;
            var timestamp = context.Request["timestamp"] ?? string.Empty;
            var nonce = context.Request["nonce"] ?? string.Empty;
            var echostr = context.Request["echostr"] ?? string.Empty;

            switch (context.Request.HttpMethod)
            {
                case "GET":
                    UpdateTest();
                    var verifyUrl = Signature.VerifyURL(Token, EncodingAESKey, CorpId, msg_signature, timestamp, nonce,
                        echostr);
                    if (verifyUrl != null)
                    {
                        context.Response.Write(verifyUrl);
                    }
                    else
                    {
                        context.Response.Write("如果你在浏览器中看到这句话，说明此地址可以被作为微信公众账号后台的Url，请注意保持Token一致。");
                    }

                    break;
                case "POST":

                    var postModel = new QyPostModel()
                    {
                        Msg_Signature = context.Request.QueryString["msg_signature"],
                        Timestamp = context.Request.QueryString["timestamp"],
                        Nonce = context.Request.QueryString["nonce"],

                        Token = Token,
                        EncodingAESKey = EncodingAESKey,
                        CorpId = CorpId,
                        Secret = Secret
                    };

                    var ent = new QyMessageHandler(context.Request.InputStream, postModel);

                    Debug.WriteLine(ent.RequestDocument);
                    var agentXS = new AgentXS();
                    agentXS.Attach(ent.Agents, 1, postModel);
                    //var agentHR = new AgentHR();
                    //agentHR.Attach(ent.Agents, 2, postModel);
                    var agentKQ2 = new AgentKQ();
                    agentKQ2.Attach(ent.Agents, 2, postModel);
                    var agentKQ = new AgentKQ();
                    agentKQ.Attach(ent.Agents, 3, postModel);
                    var agentUpload = new AgentUpload();
                    agentUpload.Attach(ent.Agents, 5, postModel);

                    ent.Execute();
                    context.Response.Write(ent.FinalResponseDocument);

                    break;
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        public void UpdateTest()
        {
  
        }
    }
}
