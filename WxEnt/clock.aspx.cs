using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using PHDS.DBUtility;

namespace QyWeixin
{
    public partial class KaoqinDaKa : System.Web.UI.Page
    {
        private DataSet GetTimeRecords(string staffId, string searchDate)
        {
            // 从EastRiver数据库读考勤记录
            string commandString = "SELECT T3.card_id AS 卡号,T3.sign_time AS 打卡时间 FROM EastRiver.DBO.TimeRecords AS T3 WHERE YEAR(T3.sign_time) = YEAR('{0}') AND MONTH(T3.sign_time) = MONTH('{0}') AND DAY(T3.sign_time) = DAY('{0}') AND EXISTS (SELECT T1.人员编号,T2.卡号,T2.状态 from PINHUA.DBO.人员档案 AS T1 INNER JOIN PINHUA.DBO.考勤卡号变动 AS T2 ON T1.ExcelServerRCID = T2.ExcelServerRCID WHERE T2.卡号=T3.card_id AND T1.人员编号='{1}')";
            commandString = string.Format(commandString, searchDate, staffId);
            // 从Pinhua的打卡登记读考勤记录
            string commandString2 = "SELECT '微信' AS 卡号, 时间 AS 打卡时间 FROM 打卡登记 AS T WHERE YEAR(T.时间) = YEAR('{0}') AND MONTH(T.时间) = MONTH('{0}') AND DAY(T.时间) = DAY('{0}') AND T.人员编号='{1}'";
            commandString2 = string.Format(commandString2, searchDate, staffId);
            // 两边记录合并，日期升序
            string commandStringUnion = "SELECT * FROM ({0} UNION ALL {1}) AS U ORDER BY 打卡时间 ASC";
            commandStringUnion = string.Format(commandStringUnion, commandString, commandString2);
            // 运行SQL命令，获取DataSet
            return SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandStringUnion);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            //string jsonString = PHDS.Weixin.QY.JsonSend.GetOpenidByCode(QyEntry.CorpId,QyEntry.Secret,Request["code"]);
            //var json = JsonConvert.DeserializeObject<OpenApi>(jsonString);
            //Response.Write(string.Format("jsonString:{0}<br>", jsonString));
            //Response.Write(string.Format("UserId:{0}<br>", json.UserId));
            //Response.Write(string.Format("OpenId:{0}<br>", json.OpenId));
            //Response.Write(string.Format("DeviceId:{0}<br>", json.DeviceId));
            //Response.Write(jsonObj["UserId"].ToString());

            if (Request.HttpMethod == "GET")
            {
                if (Request["action"] == "getstafflist")
                {
                    Debug.WriteLine(Request.RawUrl);
                    string commandString = "SELECT DISTINCT * FROM (SELECT DISTINCT T1.人员编号,T1.姓名 FROM 人员档案 AS T1 WHERE T1.状态='在职' UNION ALL SELECT DISTINCT T1.人员编号,T1.姓名 FROM 考勤明细 AS T1 WHERE YEAR(T1.日期) = YEAR('{0}') AND MONTH(T1.日期) = MONTH('{0}')) AS T ORDER BY T.人员编号 ASC";
                    commandString = string.Format(commandString, Request["date"]);
                    if (!string.IsNullOrEmpty(Request["date"]))
                    {
                        var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandString);
                        string json = JsonConvert.SerializeObject(ds.Tables[0]);

                        Response.Clear();
                        Response.Write(json);
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (Request["action"] == "gettimerecords")
                {
                    Debug.WriteLine(Request.RawUrl);
                    string sId = Request["id"];
                    string sDate = Request["date"];

                    var ds = GetTimeRecords(sId, sDate);

                    var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

                    string json = JsonConvert.SerializeObject(ds.Tables[0], timeConverter);

                    Response.Clear();
                    Response.Write(json);
                    Response.Flush();
                    Response.End();
                }
                else if (Request["action"] == "addnewrecord")
                {
                    Debug.WriteLine(Request.RawUrl);
                    string sId = Request["id"];
                    string sDate = Request["date"];
                    string sTime = Request["time"];
                    if (string.IsNullOrEmpty(sId) || string.IsNullOrEmpty(sDate) || string.IsNullOrEmpty(sTime))
                        return;
                    string commandText = "SELECT * FROM 打卡登记";
                    var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandText);
                    Debug.WriteLine(ds.Tables[0].Rows.Count);
                    var dr = ds.Tables[0].NewRow();
                    Debug.WriteLine(string.Format("'{0} {1}:00'", sDate, sTime));

                    dr["签卡原因"] = "补签";
                    dr["姓名"] = "周良";
                    dr["人员编号"] = sId;
                    dr["时间"] = string.Format("{0} {1}", sDate, sTime);
                    ds.Tables[0].Rows.Add(dr);
                    var num = SqlHelper.UpdateDataset(QyEntry.sqlConnectstr, commandText, ds, 0);
                    Debug.WriteLine(JsonConvert.SerializeObject(ds));
                    Response.Clear();
                    if (num > 0)
                        Response.Write("{\"errmsg\":\"ok\"}");
                    else
                        Response.Write("{\"errmsg\":\"fail\"}");
                    Response.Flush();
                    Response.End();
                }
                else if (Request["action"] == "deletenullrcid")
                {
                    string commandText = string.Format("DELETE FROM [Pinhua].[dbo].[打卡登记] WHERE ExcelServerRCID IS NULL AND 人员编号 = '{0}' AND CONVERT(CHAR(10),时间,120) = '{1}'", Request["id"], Request["date"]);
                    Debug.WriteLine(commandText);
                    Response.Clear();
                    try
                    {
                        var num = SqlHelper.ExecuteNonQuery(QyEntry.sqlConnectstr, CommandType.Text, commandText);
                        var error = new ErrorType
                        {
                            ErrorCode = 0,
                            ErrorMessage = "ok",
                            Json = JsonConvert.SerializeObject(
                            new
                            {
                                Count = num,
                            }),
                        };
                        Response.Write(JsonConvert.SerializeObject(error));
                    }
                    catch (SqlException ex)
                    {
                        var error = new ErrorType
                        {
                            ErrorCode = ex.ErrorCode,
                            ErrorMessage = ex.Message,
                            ErrorServer = ex.Server,
                        };
                        Response.Write(JsonConvert.SerializeObject(error));

                    }
                    finally
                    {
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (Request["action"] == "card_yuangong")
                {
                    string commandString = "SELECT T3.人员编号,T3.姓名,T1.card_id AS 卡号,T1.sign_time AS 时间 FROM EASTRIVER.DBO.TimeRecords AS T1 INNER JOIN  PINHUA.DBO.考勤卡号变动 AS T2 ON T1.card_id=T2.卡号 INNER JOIN PINHUA.DBO.人员档案 AS T3 ON T2.ExcelServerRCID=T3.ExcelServerRCID WHERE CONVERT(varchar(10), T1.sign_time, 23) = '" + Request["date"] + "' UNION ALL SELECT T2.人员编号,T2.姓名,'手动' AS 卡号,T2.时间 FROM PINHUA.DBO.打卡登记 AS T2 WHERE CONVERT(varchar(10), T2.时间, 23) = '" + Request["date"] + "' ORDER BY T3.人员编号";
                    var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandString);
                    string commandString2 = "SELECT DISTINCT 人员编号,姓名 FROM (SELECT T3.人员编号,T3.姓名,T1.card_id AS 卡号,T1.sign_time AS 时间 FROM EASTRIVER.DBO.TimeRecords AS T1 INNER JOIN  PINHUA.DBO.考勤卡号变动 AS T2 ON T1.card_id=T2.卡号 INNER JOIN PINHUA.DBO.人员档案 AS T3 ON T2.ExcelServerRCID=T3.ExcelServerRCID WHERE CONVERT(varchar(10), T1.sign_time, 23) = '" + Request["date"] + "' UNION ALL SELECT T2.人员编号,T2.姓名,'手动' AS 卡号,T2.时间 FROM PINHUA.DBO.打卡登记 AS T2 WHERE CONVERT(varchar(10), T2.时间, 23) = '" + Request["date"] + "') AS T ORDER BY 人员编号";
                    var ds2 = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandString2);
                    Response.Clear();
                    try
                    {
                        var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yy-MM-dd HH:mm" };
                        var error = new ErrorType
                        {
                            ErrorCode = 0,
                            ErrorMessage = "ok",
                            Json = JsonConvert.SerializeObject(
                            new
                            {
                                Yuangong = JsonConvert.SerializeObject(ds2.Tables[0]),
                                Shuju = JsonConvert.SerializeObject(ds.Tables[0], timeConverter),
                                Count = ds.Tables[0].Rows.Count
                            }),
                        };
                        var jsonString = JsonConvert.SerializeObject(error);
                        Response.Write(jsonString);

                    }
                    catch (SqlException ex)
                    {
                        var error = new ErrorType
                        {
                            ErrorCode = ex.ErrorCode,
                            ErrorMessage = ex.Message,
                            ErrorServer = ex.Server,
                        };
                        Response.Write(JsonConvert.SerializeObject(error));

                    }
                    finally
                    {
                        Response.Flush();
                        Response.End();
                    }
                }
                else if (Request["action"] == "WorkerList")
                {
                    Debug.WriteLine(Request.RawUrl);
                    string commandString = "SELECT DISTINCT T1.人员编号,T1.姓名 FROM 人员档案 AS T1 WHERE T1.状态='在职'ORDER BY 人员编号 ASC";

                    var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandString);
                    string json = JsonConvert.SerializeObject(ds.Tables[0]);
                    Response.Clear();
                    Response.Flush();
                    Response.End();
                }
            }

        }

    }

    public class ErrorType
    {
        public int ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorServer { get; set; }
        public string Json { get; set; }
    }
}

public class OpenApi
{
    public string UserId { get; set; }
    public string OpenId { get; set; }
    public string DeviceId { get; set; }
    public string errcode { get; set; }
    public string errmsg { get; set; }

}