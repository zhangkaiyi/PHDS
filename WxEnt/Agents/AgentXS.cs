using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Drawing;
using System.Diagnostics;
using System.Data;
using System.Data.SqlClient;
using PHDS.Weixin.QY;
using PHDS.DBUtility;
using Newtonsoft.Json.Linq;

namespace PHDS.Weixin
{
    public class AgentXS : RequestAgent
    {
        public override IResponseMessage OnEventRequest(IRequestMessage RequestMessage)
        {
            if (RequestMessage.Event == "click")
            {
                if (RequestMessage.EventKey == "最近出库")
                {
                    var dict1 = new Dictionary<string, string>();
                    var query1 = "SELECT TOP 3A.客户 as 单位,A.送货单号 as 单号,A.业务描述 as 类型, A.送货日期 as 日期 FROM 发货 A ORDER BY A.送货日期 DESC, A.客户编号 ASC";
                    var query2 = @"SELECT  A.送货单号 as 单号,B.编号,B.描述,B.PCS,B.工艺,B.木种
                                                FROM    (SELECT TOP 3 * FROM 发货 A ORDER BY A.送货日期 DESC, A.客户编号 ASC) A
                                                INNER JOIN 发货_DETAIL AS B ON A.ExcelServerRCID = B.ExcelServerRCID
                                                ORDER BY A.送货日期 DESC ,A.客户编号 ASC ,B.编号 ASC";
                    var constr = "server=www.skyflag.com,6019;database=pinhua;uid=sa;pwd=benny0922";
                    var reader1 = SqlHelper.ExecuteReader(constr, CommandType.Text, query1);
                    var reader2 = SqlHelper.ExecuteReader(constr, CommandType.Text, query2);
                    while (reader1.Read())
                    {
                        for (var i = 0; i != reader1.FieldCount; i++)
                        {
                            var sKey = reader1["单号"].ToString();
                            if (!dict1.ContainsKey(sKey))
                            {
                                dict1.Add(sKey, reader1.GetName(i) + "：" + reader1[i].ToString() + "\n");
                            }
                            else
                            {
                                dict1[sKey] += reader1.GetName(i) + "：" + reader1[i].ToString() + "\n";
                            }
                        }
                        dict1[reader1["单号"].ToString()] += "━━━━━━━━━━━\n";
                    }


                    while (reader2.Read())
                    {
                        var sKey = reader2["单号"].ToString();
                        if (dict1.ContainsKey(sKey))
                        {
                            dict1[sKey] += "◆" + "\n";

                            for (var i = 1; i != reader2.FieldCount; i++)
                            {
                                dict1[sKey] += reader2.GetName(i) + "：" + reader2[i].ToString() + "\n";
                            }
                        }
                    }
                    foreach (string s in dict1.Keys)
                    {
                        var biz = new JsonMessage
                        { touser = RequestMessage.FromUserName,
                            msgtype = "text",
                            agentid = RequestMessage.AgentID.ToString(),
                            text = new JsonMessageInnerType.textcontent { content = dict1[s] },
                            safe = 0
                        };
                        var error = JsonSend.SendQyMessage(postModel.CorpId, postModel.Secret, XLH.SerializeJsonToString(biz), Encoding.UTF8);
                        Debug.Write(error);
                    }
                }
            }
            return null;
        }
        public override IResponseMessage OnTextRequest(IRequestMessage requestMessage)
        {
            var art1 = new ResponseMessageInnerType.Article
            { Title = "ESAP第十四弹 手把手教你玩转ES微信开发",
                Description = "来自村长的ESAP系统最新技术分享。",
                PicUrl = "http://iesap.net/wp-content/uploads/2015/12/esap3-1.jpg",
                Url = "http://iesap.net/index.php/2015/12/28/esap14/"
            };
            var art2 = new ResponseMessageInnerType.Article
            { Title = "打通信息化的“任督二脉”(二)",
                Description = "来自村长的ESAP2.0系统技术分享。",
                PicUrl = "http://iesap.net/wp-content/uploads/2015/12/taiji.jpg",
                Url = "http://iesap.net/index.php/2015/12/16/esap2-1/"
            };
            var art3 = new ResponseMessageInnerType.Article
            { Title = "打通信息化的“任督二脉”(一)",
                Description = "来自村长的ESAP2.0系统技术分享。",
                PicUrl = "http://iesap.net/wp-content/uploads/2015/12/rdem.jpg",
                Url = "http://iesap.net/index.php/2015/12/11/esap2-0/"
            };

            var responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.News);
            responseMessage.Articles.Add(art1);
            responseMessage.Articles.Add(art2);
            responseMessage.Articles.Add(art3);
            return responseMessage;
        }
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
