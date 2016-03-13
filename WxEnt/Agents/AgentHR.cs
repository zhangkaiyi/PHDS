using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using PHDS.Weixin.QY;

namespace PHDS.Weixin
{
    public class AgentHR : RequestAgent
    {
        public override IResponseMessage OnTextRequest(IRequestMessage requestMessage)
        {
            var biz = new JsonMessage
            { touser = requestMessage.FromUserName,
                msgtype = "text",
                agentid = requestMessage.AgentID.ToString(),
                text = new JsonMessageInnerType.textcontent { content = requestMessage.Content },
                safe = 0
            };
            var error = JsonSend.SendQyMessage(postModel.CorpId, postModel.Secret, XLH.SerializeJsonToString(biz), Encoding.UTF8);
            Debug.WriteLine(error);
            ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);

            return null;
        }
    }
}
