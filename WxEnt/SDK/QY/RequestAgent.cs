using System;
using System.Collections.Generic;
using System.Web;

namespace PHDS.Weixin.QY
{
    public class RequestAgent
    {
        public QyPostModel postModel { get; set; }
        public int AgentID { get; set; }
        public bool Attach(Dictionary<int,RequestAgent> agents, int agentid, QyPostModel postModel)
        {
            if (agents.ContainsKey(agentid) || agents == null)
            {
                return false;
            }
            else
            {
                agents.Add(agentid, this);
                this.postModel = postModel;
                return true;
            }
        }
        public virtual IResponseMessage OnTextRequest(IRequestMessage requestMessage) 
        { 
            System.Diagnostics.Debug.WriteLine("Invoke OnTextRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnImageRequest(IRequestMessage requestMessage)
        { 
            System.Diagnostics.Debug.WriteLine("Invoke OnImageRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnVoiceRequest(IRequestMessage requestMessage) 
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnVoiceRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnShortVoiceRequest(IRequestMessage requestMessage)
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnShortVoiceRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnVideoRequest(IRequestMessage requestMessage)
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnVideoRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnLocationRequest(IRequestMessage requestMessage)
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnLocationRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnLinkRequest(IRequestMessage requestMessage)
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnLinkRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        public virtual IResponseMessage OnEventRequest(IRequestMessage requestMessage) 
        {
            System.Diagnostics.Debug.WriteLine("Invoke OnEventRequest.");
            return DefaultResponseMessage(requestMessage);
        }
        /// <summary>
        /// 默认返回消息（当任何OnXX消息没有被重写，都将自动返回此默认消息）
        /// </summary>
        public virtual IResponseMessage DefaultResponseMessage(IRequestMessage requestMessage)
        {
            return new ResponseMessage { }; 
        }
    }
}