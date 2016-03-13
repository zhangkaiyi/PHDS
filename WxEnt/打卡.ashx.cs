using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Data.SqlClient;
using QyWeixin.EF;

namespace QyWeixin
{
    /// <summary>
    /// 打卡 的摘要说明
    /// </summary>
    public class 打卡 : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";

            if (context.Request.HttpMethod == "GET")
                ProcessGet(context);
            else if (context.Request.HttpMethod == "POST")
                ProcessPost(context);
        }

        private void ProcessGet(HttpContext context)
        {
            switch (context.Request["action"])
            {
                case "getWorkerList":

                    using (var pinhua = new PinhuaEntities())
                    {
                        var info = from row in pinhua.人员档案
                                   where row.状态 == "在职"
                                   orderby row.人员编号 ascending
                                   select new
                                   {
                                       row.人员编号,
                                       row.姓名
                                   };
                        string json = JsonConvert.SerializeObject(info);
                        context.Response.Write(json);
                    }
                    break;

                case "getRecordTimeToday":
                    {

                        var date = Convert.ToDateTime(context.Request["date"]);

                        using (var pinhua = new PinhuaEntities())
                        using (var eastriver = new EastRiverEntities())
                        {
                            var eastriverinfo = from row in eastriver.TimeRecords
                                                where row.sign_time.Year == date.Year && row.sign_time.Month == date.Month && row.sign_time.Day == date.Day
                                                select row;

                            var pinhuainfo = from row in pinhua.人员档案
                                             join row2 in pinhua.考勤卡号变动
                                                 on row.ExcelServerRCID equals row2.ExcelServerRCID
                                             select new
                                             {
                                                 row.人员编号,
                                                 row.姓名,
                                                 row2.卡号,
                                             };

                            var info = from x in pinhuainfo.ToList()
                                       join y in eastriverinfo.ToList() on x.卡号 equals y.card_id
                                       select new
                                       {
                                           x.人员编号,
                                           x.姓名,
                                           x.卡号,
                                           时间 = y.sign_time
                                       };

                            var workerlist = (from p in info
                                              select new
                                              {
                                                  p.人员编号,
                                                  p.姓名
                                              }
                                ).Distinct();

                            try
                            {
                                var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd HH:mm" };
                                var error = new ErrorType
                                {
                                    ErrorCode = 0,
                                    ErrorMessage = "ok",
                                    Json = JsonConvert.SerializeObject(
                                    new
                                    {
                                        Yuangong = JsonConvert.SerializeObject(workerlist),
                                        Shuju = JsonConvert.SerializeObject(info, timeConverter),
                                        Count = info.Count()
                                    }),
                                };
                                var jsonString = JsonConvert.SerializeObject(error);
                                context.Response.Write(jsonString);

                            }
                            catch (SqlException ex)
                            {
                                var error = new ErrorType
                                {
                                    ErrorCode = ex.ErrorCode,
                                    ErrorMessage = ex.Message,
                                    ErrorServer = ex.Server,
                                };
                                context.Response.Write(JsonConvert.SerializeObject(error));

                            }
                        }

                    }
                    break;
            }
        }

        private void ProcessPost(HttpContext context)
        {
            switch (context.Request["action"])
            {
                case "postTimeInfo":

                    using (var pinhua = new PinhuaEntities())
                    {
                        var id = context.Request.Form["id"];
                        var name = context.Request.Form["name"];
                        var time = Convert.ToDateTime(context.Request.Form["time"]);
                        var rcid = "wx" + DateTime.Now.ToString("yyyyMMdd");
                        var xxx = from p in pinhua.ES_RepCase
                                  where p.rcId.Substring(0, 10) == rcid
                                  select p;
                        var yyy = from p in xxx
                                  orderby p.rcId.Substring(10) descending
                                  select p;

                        if (yyy.Count() == 0)
                        {
                            rcid += "00001"; 
                        }
                        else
                        {
                            var zzz = (int.Parse(string.IsNullOrEmpty(yyy.FirstOrDefault().rcId.Substring(10)) ? "0" : yyy.FirstOrDefault().rcId.Substring(10)) + 1).ToString("D5");
                            rcid += zzz;
                        }

                        var data_RepCase = new ES_RepCase
                        {
                            rcId = rcid,
                            RtId = "74.1",
                            lstFiller = 2,
                            lstFillerName = "张凯译",
                            lstFillDate = DateTime.Now,
                            fillDate = DateTime.Now,
                            wiId = "",
                            state = 1,
                        };

                        var data = new 打卡登记
                        {
                            ExcelServerRCID = rcid,
                            ExcelServerRTID = "74.1",
                            签卡原因 = "微信",
                            人员编号 = id,
                            姓名 = name,
                            时间 = time,
                        };

                        pinhua.ES_RepCase.Add(data_RepCase);
                        pinhua.打卡登记.Add(data);
                        try
                        {
                            var num = pinhua.SaveChanges();

                        }
                        catch (Exception ex)
                        {

                            Debug.WriteLine(ex.InnerException.InnerException.Message);
                        }
                    }
                    break;
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