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
    public partial class clockin : System.Web.UI.Page
    {
        private DataSet GetTimeRecords(string staffId, string searchDate)
        {
            // 从EastRiver数据库读考勤记录
            string commandString = "SELECT T3.card_id AS 卡号,T3.sign_time AS 打卡时间 FROM EastRiver.DBO.TimeRecords AS T3 WHERE YEAR(T3.sign_time) = YEAR('{0}') AND MONTH(T3.sign_time) = MONTH('{0}') AND DAY(T3.sign_time) = DAY('{0}') AND EXISTS (SELECT T1.人员编号,T2.卡号,T2.状态 from PINHUA.DBO.人员档案 AS T1 INNER JOIN PINHUA.DBO.考勤卡号变动 AS T2 ON T1.ExcelServerRCID = T2.ExcelServerRCID WHERE T2.卡号=T3.card_id AND T1.人员编号='{1}')";
            commandString = string.Format(commandString, searchDate, staffId);
            // 从Pinhua的打卡登记读考勤记录
            string commandString2 = "SELECT '手动' AS 卡号, 时间 AS 打卡时间 FROM 打卡登记 AS T WHERE YEAR(T.时间) = YEAR('{0}') AND MONTH(T.时间) = MONTH('{0}') AND DAY(T.时间) = DAY('{0}') AND T.人员编号='{1}'";
            commandString2 = string.Format(commandString2, searchDate, staffId);
            // 两边记录合并，日期升序
            string commandStringUnion = "SELECT * FROM ({0} UNION ALL {1}) AS U ORDER BY 打卡时间 ASC";
            commandStringUnion = string.Format(commandStringUnion, commandString, commandString2);
            // 运行SQL命令，获取DataSet
            return SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandStringUnion);
        }

        private void BindToGridView(GridView gridView, string sId, string sDatetime)
        {
            // 从EastRiver数据库读考勤记录
            string commandString = "SELECT T3.card_id AS 卡号,T3.sign_time AS 打卡时间 FROM EastRiver.DBO.TimeRecords AS T3 WHERE YEAR(T3.sign_time) = YEAR('{0}') AND MONTH(T3.sign_time) = MONTH('{0}') AND DAY(T3.sign_time) = DAY('{0}') AND EXISTS (SELECT T1.人员编号,T2.卡号,T2.状态 from PINHUA.DBO.人员档案 AS T1 INNER JOIN PINHUA.DBO.考勤卡号变动 AS T2 ON T1.ExcelServerRCID = T2.ExcelServerRCID WHERE T2.卡号=T3.card_id AND T1.人员编号='{1}')";
            commandString = string.Format(commandString, sDatetime, sId);
            // 从Pinhua的打卡登记读考勤记录
            string commandString2 = "SELECT '手动' AS 卡号, 时间 AS 打卡时间 FROM 打卡登记 AS T WHERE YEAR(T.时间) = YEAR('{0}') AND MONTH(T.时间) = MONTH('{0}') AND DAY(T.时间) = DAY('{0}') AND T.人员编号='{1}'";
            commandString2 = string.Format(commandString2, sDatetime, sId);
            // 两边记录合并，日期升序
            string commandStringUnion = "SELECT * FROM ({0} UNION ALL {1}) AS U ORDER BY 打卡时间 ASC";
            commandStringUnion = string.Format(commandStringUnion, commandString, commandString2);
            // 运行SQL命令，获取DataSet
            var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandStringUnion);
            // Data绑定GridView，输出数据
            gridView.DataSource = ds;
            gridView.DataBind();
            // 如果返回的是空集，结束
            if (ds.Tables[0].Rows.Count == 0)
                return;
            // 给GridView加<thead></thead>，css控制表头风格，如果DataSet空集会出错，所以在上面做判断
            //GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
        }

         private void BindToUserinfoList(DropDownList dropdownList, string sDatetime)
        {
            string commandString = "SELECT DISTINCT * FROM (SELECT DISTINCT T1.人员编号,T1.姓名 FROM 人员档案 AS T1 WHERE T1.状态='在职' UNION ALL SELECT DISTINCT T1.人员编号,T1.姓名 FROM 考勤明细 AS T1 WHERE YEAR(T1.日期) = YEAR('{0}') AND MONTH(T1.日期) = MONTH('{0}')) AS T ORDER BY T.人员编号 ASC";
            commandString = string.Format(commandString, sDatetime);
            var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, CommandType.Text, commandString);
            dropdownList.DataSource = ds.Tables[0];
            dropdownList.DataTextField = "姓名";
            dropdownList.DataValueField = "人员编号";
            dropdownList.DataBind();
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
                if(Request["action"] == "getstafflist")
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
                else if(Request["action"] == "gettimerecords")
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
                
            }
        }

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