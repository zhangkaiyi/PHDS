using System;
using System.Collections.Generic;
using System.Web;
using System.Diagnostics;
using PHDS.DBUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QyWeixin
{
    /// <summary>
    /// jqGrid 的摘要说明
    /// </summary>
    public class jqGrid : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            if (context.Request.HttpMethod == "GET")
            {
                if (context.Request["query"] == "jqGrid")
                {
                    var dateStart = context.Request["start"] ?? string.Empty;
                    var dateEnd = context.Request["end"] ?? string.Empty;
                    var numberOfRows = context.Request["rows"];
                    var pageIndex = context.Request["page"];
                    var sortColumnName = context.Request["sidx"];
                    var sortOrderBy = context.Request["sord"];

                    var cols = new string[] {   "单号",
                                        "物品编号",
                                        "日期",
                                        "单位",
                                        "描述",
                                        "PCS",
                                        "工艺",
                                        "木种",
                                        "规格",
                                        "计价单位",
                                        "单位数量",
                                        "金额"
                    };
                    var sqlSelect1 = @"
                            SELECT  A.送货单号 AS {0} ,
                            B.编号 AS {1} ,
                            A.送货日期 AS {2} ,
                            A.客户 AS {3} ,
                            B.描述 AS {4} ,
                            CAST(B.PCS AS INT) AS {5},
                            B.工艺 AS {6} ,
                            B.木种 AS {7},
                            B.规格 AS {8},
                            B.计价单位 AS {9},
                            CAST(B.单位数量 AS DECIMAL(8,2)) AS {10},
                            B.金额 AS {11}
                            FROM    ( SELECT *
                            FROM      发货 A WHERE 送货日期 BETWEEN '{12}' AND '{13}'
                            ) A
                            INNER JOIN 发货_DETAIL AS B ON A.ExcelServerRCID = B.ExcelServerRCID ";
                    sqlSelect1 = string.Format(sqlSelect1, cols[0], cols[1], cols[2], cols[3], cols[4], cols[5], cols[6], cols[7], cols[8], cols[9], cols[10], cols[11], dateStart, dateEnd);
                    var sqlOrderby = "ORDER BY ";
                    if (string.IsNullOrEmpty(sortColumnName))
                    {
                        sqlOrderby = string.Empty;
                    }
                    else
                    {
                        if (sortColumnName.EndsWith(", "))
                        {
                            sqlOrderby = sqlOrderby + sortColumnName.TrimEnd(' ', ',');
                        }
                        else
                        {
                            sqlOrderby = sqlOrderby + sortColumnName + " " + sortOrderBy;
                        }
                    }
                    var sqlSelect2 = "SELECT * FROM (SELECT *, ROW_NUMBER() OVER (ORDER BY 日期 DESC, 物品编号 ASC) AS RN FROM ({1}) AS A) AS B WHERE RN BETWEEN ({2}-1)*{3}+1 AND ({2}-1)*{3}+1+{3} " + sqlOrderby;
                    sqlSelect2 = string.Format(sqlSelect2, sqlOrderby, sqlSelect1, pageIndex, numberOfRows);

                    var connectstring = "server=www.skyflag.com,6019;database=pinhua;uid=sa;pwd=benny0922";

                    var ds = SqlHelper.ExecuteDataset(connectstring, System.Data.CommandType.Text, sqlSelect2);

                    var temp = "SELECT COUNT(1) FROM (" + sqlSelect1 + ") AS TEMP";
                    var numberofRecords = (int)SqlHelper.ExecuteScalar(connectstring, System.Data.CommandType.Text, temp);

                    context.Response.Clear();
















                    var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                    timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";

                    var jsonObj = JObject.Parse(JsonConvert.SerializeObject(ds, timeConverter));
                    jsonObj.Add(new JProperty("records", numberofRecords));
                    jsonObj.Add(new JProperty("total", Math.Ceiling((decimal)numberofRecords / decimal.Parse(numberOfRows))));
                    context.Response.Write(jsonObj.ToString());
                }
            }
            else
            {
                if (context.Request.HttpMethod == "POST")
                {
                    if (context.Request["query"] == "gongzi")
                    {
                        var numberOfRows = context.Request["rows"];

                        var sqlConnectstr = "server=www.skyflag.com,6019;database=pinhua;uid=sa;pwd=benny0922";

                        var rcid = context.Request["rcid"];
                        var id = context.Request["id"];
                        if (string.IsNullOrEmpty(rcid) || string.IsNullOrEmpty(id))
                        {
                            var sqlSelect1 = "SELECT DISTINCT CAST(T1.年份 AS nvarchar)+'-'+CASE WHEN T1.月份 < 10 THEN '0' ELSE '' END +CAST(T1.月份 AS nvarchar)  AS 日期,T1.ExcelServerRCID,T2.编号,T2.姓名,T2.计算金额 AS 金额 FROM test_主表 AS T1 INNER JOIN test_明细 AS T2 ON T1.ExcelServerRCID=T2.ExcelServerRCID WHERE T2.工资项名称 = '抹零' ORDER BY 日期 DESC,T2.编号 ASC";
                            var ds = SqlHelper.ExecuteDataset(sqlConnectstr, System.Data.CommandType.Text, sqlSelect1);
                            var temp = "SELECT COUNT(1) FROM (SELECT DISTINCT ExcelServerRCID,姓名 FROM test_明细) AS T";
                            var numberofRecords = (int)SqlHelper.ExecuteScalar(sqlConnectstr, System.Data.CommandType.Text, temp);
                            var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                            var jsonObj = JObject.Parse(JsonConvert.SerializeObject(ds, timeConverter));
                            jsonObj.Add(new JProperty("records", numberofRecords));
                            jsonObj.Add(new JProperty("total", Math.Ceiling((decimal)numberofRecords / decimal.Parse(numberOfRows))));
                            JsonConvert.SerializeObject(ds);
                            context.Response.Clear();
                            context.Response.Write(jsonObj);
                        }
                        else
                        {
                            var sqlSelect1 = "SELECT DISTINCT * FROM test_明细 AS T2 WHERE T2.ExcelServerRCID = '{0}' AND 编号 = '{1}'";
                            sqlSelect1 = string.Format(sqlSelect1, rcid, id);
                            var ds = SqlHelper.ExecuteDataset(sqlConnectstr, System.Data.CommandType.Text, sqlSelect1);
                            var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter();
                            timeConverter.DateTimeFormat = "yyyy'-'MM'-'dd";
                            var jsonString = JsonConvert.SerializeObject(ds, timeConverter);
                            Debug.WriteLine(jsonString);
                            context.Response.Clear();
                            context.Response.Write(jsonString);
                        }
                    }
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
