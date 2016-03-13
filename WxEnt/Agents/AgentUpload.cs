using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Drawing;
using PHDS.Weixin.QY;
using Newtonsoft.Json.Linq;

namespace PHDS.Weixin
{
    public class AgentUpload : RequestAgent
    {
        public override IResponseMessage OnImageRequest(IRequestMessage requestMessage)
        {
            HttpWebResponse res = null;

            var sDate = DateTime.Now.ToString("yyyyMMdd");
            var sPath = System.AppDomain.CurrentDomain.BaseDirectory + "/Pic/" + sDate;

            var jsonObj = JObject.Parse(QY.JsonSend.GetQyUserinfo(postModel.CorpId, postModel.Secret, requestMessage.FromUserName));
            var sUserName = jsonObj["name"].Value<string>();
            var sFileTime = DateTime.Now.ToFileTime().ToString();
            var sFileName = sUserName + sFileTime;

            var responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);
            responseMessage.Content = "保存成功，文件名：" + sFileName + ".jpg";

            try
            {
                var httpUrl = new System.Uri(requestMessage.PicUrl);
                var req = (HttpWebRequest)(WebRequest.Create(httpUrl));
                req.Timeout = 180000;
                req.Method = "GET";
                res = (HttpWebResponse)(req.GetResponse());
                var img = new Bitmap(res.GetResponseStream());

                if (Directory.Exists(sPath) == false)
                {
                    Directory.CreateDirectory(sPath);
                }
                img.Save(sPath + "/" + sFileName + ".jpg");
            }

            catch (Exception ex)
            {
                var aa = ex.Message;
                responseMessage.Content = aa;
            }
            finally
            {
                res.Close();
            }
            return responseMessage;
        }
    }
}
