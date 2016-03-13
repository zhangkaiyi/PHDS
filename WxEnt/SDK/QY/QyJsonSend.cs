using System;
using System.Collections.Generic;
using System.Web;
using System.Net;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace PHDS.Weixin.QY
{
    public class JsonSend
    {
        /// <summary>
        /// 获取企业号的accessToken
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public static string GetQyAccessToken(string corpid, string corpsecret)
        {
            string getAccessTokenUrl = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={0}&corpsecret={1}";
            string accessToken = "";

            string respText = "";

            //获取josn数据
            string url = string.Format(getAccessTokenUrl, corpid, corpsecret);

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            using (Stream resStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(resStream, Encoding.UTF8);
                respText = reader.ReadToEnd();
                resStream.Close();
            }

            try
            {
                Dictionary<string, object> respDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(respText);

                //通过键access_token获取值
                accessToken = respDic["access_token"].ToString();
            }
            catch (Exception) { }

            return accessToken;
        }

        /// <summary>
        /// 创建菜单
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public static string CreateQyMenu(string corpid, string corpsecret, int agentid, string menuData, Encoding dataEncode)
        {
            string accessToken = GetQyAccessToken(corpid, corpsecret);
            string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/menu/create?access_token={0}&agentid={1}", accessToken, agentid);
            return PostWebRequest(postUrl, menuData, dataEncode);

        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public static string GetQyMenu(string corpid, string corpsecret, int agentid)
        {
            string accessToken = GetQyAccessToken(corpid, corpsecret);
            string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/menu/get?access_token={0}&agentid={1}", accessToken, agentid);
            return GetWebRequest(postUrl);

        }

        /// <summary>
        /// 获取菜单
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public static string GetOpenidByCode(string corpid, string corpsecret, string code)
        {
            string accessToken = GetQyAccessToken(corpid, corpsecret);
            string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/getuserinfo?access_token={0}&code={1}", accessToken, code);
            return GetWebRequest(postUrl);

        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <returns></returns>
        public static string GetQyUserinfo(string corpid, string corpsecret, string userid)
        {
            string accessToken = GetQyAccessToken(corpid, corpsecret);
            string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/user/get?access_token={0}&userid={1}", accessToken, userid);
            return GetWebRequest(postUrl);

        }

        /// <summary>
        /// Post数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData, Encoding dataEncode)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = dataEncode.GetBytes(paramData); //转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";

                webReq.ContentLength = byteArray.Length;
                Stream newStream = webReq.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);//写入参数
                newStream.Close();
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
                newStream.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// Get数据接口
        /// </summary>
        /// <param name="postUrl">接口地址</param>
        /// <param name="paramData">提交json数据</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public static string GetWebRequest(string postUrl)
        {
            string ret = string.Empty;
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "GET";
                webReq.ContentType = "application/x-www-form-urlencoded";

                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                ret = sr.ReadToEnd();
                sr.Close();
                response.Close();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return ret;
        }

        /// <summary>
        /// 推送信息
        /// </summary>
        /// <param name="corpid">企业号ID</param>
        /// <param name="corpsecret">管理组密钥</param>
        /// <param name="paramData">提交的数据json</param>
        /// <param name="dataEncode">编码方式</param>
        /// <returns></returns>
        public static string SendQyMessage(string corpid, string corpsecret, string paramData, Encoding dataEncode)
        {
            string accessToken = GetQyAccessToken(corpid, corpsecret);
            string postUrl = string.Format("https://qyapi.weixin.qq.com/cgi-bin/message/send?access_token={0}", accessToken);
            return PostWebRequest(postUrl, paramData, dataEncode);
        }
    }

    public class JsonMessage
    {
        public string touser { get; set; }      //`json:"touser"`
        public string toparty { get; set; }     //`json:"toparty"`
        public string totag { get; set; }       //`json:"totag"`
        public string msgtype { get; set; }     //`json:"msgtype"`
        public string agentid { get; set; }     //`json:"agentid"`
        public JsonMessageInnerType.textcontent text { get; set; }       //`json:"text"`
        public JsonMessageInnerType.media image { get; set; }        //`json:"image"`
        public JsonMessageInnerType.media voice { get; set; }        //`json:"voice"`
        public JsonMessageInnerType.video video { get; set; }        //`json:"video"`
        public JsonMessageInnerType.media file { get; set; }         //`json:"file"`
        public JsonMessageInnerType.News news { get; set; }        //`json:"news"`
        public JsonMessageInnerType.MpNews mpnews { get; set; }    //`json:"mpnews"`
        public int safe { get; set; }            //`json:"safe"`
    }
    public class JsonMessageInnerType
    {
        public class textcontent //企业消息-文本
        {
            public string content { get; set; }//`json:"content"`
        }
        //企业消息-媒体
        public class media
        {
            public string media_id { get; set; } // `json:"media_id"`
        }
        //企业消息-视频
        public class video
        {
            public string media_id { get; set; }     //`json:"media_id"`
            public string title { get; set; }        //`json:"title"`
            public string description { get; set; }  //`json:"description"`
        }
        public class News
        {
            public New[] articles { get; set; }
        }
        //企业消息-单图文
        public class New
        {
            public string title { get; set; }       //`json:"title"`
            public string description { get; set; }  //`json:"description"`
            public string url { get; set; }      //`json:"url"`
            public string picurl { get; set; }      //`json:"picurl"`
        }
        //企业消息-密图文组
        public class MpNews
        {
            public MpNew articles { get; set; }// `json:"articles"`
        }
        public class MpNew
        {
            public string title { get; set; }        //`json:"title"`
            public string thumb_media_id { get; set; }  //`json:"thumb_media_id`
            public string author { get; set; }      //`json:"author"`
            public string content_source_url { get; set; }         //`json:"content_source_url"`
            public string content { get; set; }      //`json:"content"`
            public string digest { get; set; }       //`json:"digest"`
            public string show_cover_pic { get; set; }  //`json:"show_cover_pic"`
        }
    }
}