using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Diagnostics;
using PHDS.DBUtility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace QyWeixin
{
    public partial class _Kaoqin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var sID = Request["id"] ?? string.Empty;
            var sDate = Request["date"] ?? string.Empty;
            var sName = Request["name"] ?? string.Empty;

            if (string.IsNullOrEmpty(sID) || string.IsNullOrEmpty(sDate))
            {
                return;
            }
            Label1.Text = sName;
            Label2.Text = string.Format("{0}年{1}月", sDate.Substring(0, 4), sDate.Substring(4, 2));

            var sqlSelect = "SELECT SUBSTRING(CONVERT(varchar(10), 日期, 112),7,2) AS 日,状态,上班1+CASE WHEN 上班1 <>'' OR 下班1 <>'' THEN '～' ELSE ''END+下班1 AS 上午,上班2+CASE WHEN 上班2 <>'' OR 下班2 <>'' THEN '～' ELSE ''END+下班2 AS 下午,上班3+CASE WHEN 上班3 <>'' OR 下班3 <>'' THEN '～' ELSE ''END+ 下班3 AS 晚上,CAST(ISNULL(正常出勤,0)+ISNULL(加班,0) AS DECIMAL(5,1)) AS 出勤 FROM 考勤期间 AS T1 INNER JOIN 考勤明细 AS T2 ON T1.ExcelServerRCID=T2.ExcelServerRCID WHERE T1.年=SUBSTRING('{1}',1,4) AND T1.月=SUBSTRING('{1}',5,2) AND T2.人员编号='{0}' ORDER BY 日期";
            sqlSelect = string.Format(sqlSelect, sID, sDate);
            var ds = SqlHelper.ExecuteDataset(QyEntry.sqlConnectstr, System.Data.CommandType.Text, sqlSelect);
            string sAction = Request["action"];
            //if (Request.HttpMethod == "GET")
            //{
            //    Debug.WriteLine(Request.RawUrl);
            //    if (sAction == "getkaoqin")
            //    {
            //        Response.Clear();
            //        Response.Write(JsonConvert.SerializeObject(ds));
            //        Response.Flush();
            //        Response.End();
            //    }
            //}
            if (ds.Tables[0].Rows.Count == 0)
            {
                return;
            }

            var hidden480 = "hidden-480";
            GridView1.UseAccessibleHeader = true;
            GridView1.DataSource = ds;
            GridView1.DataBind();
            GridView1.HeaderRow.TableSection = TableRowSection.TableHeader;
            GridView1.HeaderRow.Cells[1].CssClass = hidden480;

            foreach (GridViewRow i in GridView1.Rows)
            {
                i.Cells[1].CssClass = hidden480;
            }


        }
        /// <summary> 
        /// 合并GridView中某列相同信息的行（单元格） 
        /// </summary>
        /// <param name="GridView1">GridView</param>
        /// <param name="cellNum">第几列</param>
        public static void GroupRows(GridView GridView1, int cellNum)
        {
            var i = 0;
            var rowSpanNum = 1;
            while (i < GridView1.Rows.Count - 1)
            {
                var gvr = GridView1.Rows[i];

                for (++i; i < GridView1.Rows.Count; i++)
                {
                    var gvrNext = GridView1.Rows[i];
                    if (gvr.Cells[cellNum].Text == gvrNext.Cells[cellNum].Text)
                    {
                        gvrNext.Cells[cellNum].Visible = false;
                        rowSpanNum++;
                    }
                    else
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                        rowSpanNum = 1;
                        break;
                    }

                    if (i == GridView1.Rows.Count - 1)
                    {
                        gvr.Cells[cellNum].RowSpan = rowSpanNum;
                    }
                }
            }
        }
    }
}
