using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Data;
using PHDS.Weixin.QY;
using PHDS.DBUtility;
using Newtonsoft.Json.Linq;

namespace PHDS.Weixin
{
    public class AgentKQ : RequestAgent
    {
        public override IResponseMessage OnEventRequest(IRequestMessage requestMessage)
        {
            IResponseMessage responseMessage = null;
            switch (requestMessage.EventKey)
            {
                case "考勤数据":
                    {
                        var jsonObj = JObject.Parse(JsonSend.GetQyUserinfo(postModel.CorpId, postModel.Secret, requestMessage.FromUserName));

                        responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);
                        var sSelect = "SELECT DISTINCT TOP 1 CAST(T1.年 AS nvarchar) + CASE WHEN T1.月<10 THEN '0' ELSE '' END + CAST(T1.月 AS nvarchar) AS 日期 FROM 考勤期间 AS T1 INNER JOIN 考勤明细 AS T2 ON T1.ExcelServerRCID = T2.ExcelServerRCID INNER JOIN (SELECT 人员编号 as 'id' FROM 人员档案 AS T1 WHERE T1.证件号码 = '330421198112112514') AS T3 ON T2.人员编号 = T3.id ORDER BY 日期 DESC";
                        var dr = SqlHelper.ExecuteReader(QyWeixin.QyEntry.sqlConnectstr, CommandType.Text, sSelect);
                        var s = "";
                        while (dr.Read())
                        {
                            s += dr["日期"];
                        }
                        responseMessage.Content = string.Format("欢迎使用企业号，{0}\n您现在最新可查询的考勤批次是：{1}\n请在输入框中以如上格式输入您要查询的考勤批次", jsonObj["name"], s);

                        //var biz = new JsonMessage
                        //{
                        //    touser = requestMessage.FromUserName,
                        //    msgtype = "text",
                        //    agentid = requestMessage.AgentID.ToString(),
                        //    text = new JsonMessageInnerType.textcontent { content = responseMessage.Content },
                        //    safe = 0
                        //};
                        //var error = JsonSend.SendQyMessage(postModel.CorpId, postModel.Secret, XLH.SerializeJsonToString(biz), System.Text.Encoding.UTF8);
                    }
                    break;
                case "手动打卡":
                    {
                        var jsonObj = JObject.Parse(JsonSend.GetQyUserinfo(postModel.CorpId, postModel.Secret, requestMessage.FromUserName));
                        Debug.WriteLine(jsonObj["department"].ToString());
                        var jsonArray = (JArray)jsonObj["department"];

                        foreach (var item in jsonArray)
                        {
                            if((int)item != 2 && (int)item !=6)
                            {
                                responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);
                                responseMessage.Content = "你没有权限操作，请联系负责人补打。";
                                
                            }
                            else
                            {
                                responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.News);
                                string surl = "https://open.weixin.qq.com/connect/oauth2/authorize?appid=wx87c90793c5376e09&redirect_uri={0}&response_type=code&scope=snsapi_base&state=STATE#wechat_redirect";
                                string encodeurl = System.Web.HttpUtility.UrlEncode("http://senderwood.vicp.net/clock.aspx");
                                Debug.WriteLine(string.Format(surl, encodeurl));
                                responseMessage.Articles.Add(new ResponseMessageInnerType.Article
                                {
                                    Url = "http://pinhuadashi.ticp.net/打卡.html",
                                    Description = "点击进行操作",
                                    Title = "手动打卡"
                                });
                                
                            }
                        }

                    }
                    break;
            }

            return responseMessage;
        }
        public override IResponseMessage OnTextRequest(IRequestMessage requestMessage)
        {
            IResponseMessage responseMessage = null;

            DateTime retdate;
            IFormatProvider ifp = new System.Globalization.CultureInfo("zh-CN", true);
            var bDate = DateTime.TryParseExact(requestMessage.Content, "yyyyMM", ifp, System.Globalization.DateTimeStyles.None, out retdate);


            if (requestMessage.Content.Length != 6 || !bDate)
            {
                responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);
                responseMessage.Content = "您输入的格式有误。";
                return responseMessage;
            }

            var jsonObj = JObject.Parse(JsonSend.GetQyUserinfo(postModel.CorpId, postModel.Secret, requestMessage.FromUserName));
            var sUserid = (string)jsonObj["userid"];
            Debug.WriteLine(sUserid);
            if (sUserid == "1")
                sUserid = "330421199506040511";

            var sSelect = "SELECT * FROM 考勤期间 AS T1 INNER JOIN 考勤汇总_明细 AS T2 ON T1.ExcelServerRCID=T2.ExcelServerRCID INNER JOIN (SELECT 人员编号 as 'id' FROM 人员档案 AS T1 WHERE T1.证件号码 = '{0}') AS T3 ON T3.id = T2.人员编号 WHERE T1.年=SUBSTRING('{1}',1,4) AND T1.月=SUBSTRING('{1}',5,2)";
            sSelect = string.Format(sSelect, sUserid, requestMessage.Content);

            var ds = SqlHelper.ExecuteDataset(QyWeixin.QyEntry.sqlConnectstr, CommandType.Text, sSelect);
            if (ds.Tables[0].Rows.Count == 0)
            {
                Debug.WriteLine(sSelect);
                responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.Text);
                responseMessage.Content = "查询不到关于你的数据。";
                return responseMessage;
            }
            var sID = (string)ds.Tables[0].Rows[0]["id"];
            var sName = (string)ds.Tables[0].Rows[0]["姓名"];
            var sStardard = Math.Round((decimal)ds.Tables[0].Rows[0]["标准工时"], 1);
            var sReal = Math.Round((decimal)ds.Tables[0].Rows[0]["实出勤"], 1);
            var sJiaban = sReal - sStardard > 0 ? sReal - sStardard : 0;
            var sQuanqin = (string)ds.Tables[0].Rows[0]["是否全勤"];
            var sDescription = "{0}，该月标准工时{1}小时，您实际出勤{2}小时，其中{3}小时将作为加班工时为您结算加班工资。全勤：{4}";
            var sUrl = "http://pinhuadashi.ticp.net/kaoqin.aspx?id={0}&date={1}&name={2}";
            var sDate = requestMessage.Content;
            sUrl = string.Format(sUrl, sID, sDate, sName);
            sDescription = string.Format(sDescription, sName, sStardard, sReal, sJiaban, sQuanqin);
            Debug.WriteLine(sUserid);
            Debug.WriteLine(sSelect);
            responseMessage = ResponseMessage.CreateFromRequestMessage(requestMessage, QY.Enums.ResponseType.News);
            var art1 = new ResponseMessageInnerType.Article
            { Title = requestMessage.Content + "考勤数据",
                Description = sDescription,
                Url = sUrl,
              PicUrl = "http://pinhuadashi.ticp.net/Assets/salary.jpg"
            };
            responseMessage.Articles.Add(art1);
            return responseMessage;
        }
    }
}
