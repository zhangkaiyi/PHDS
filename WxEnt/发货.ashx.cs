using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Diagnostics;
using Newtonsoft.Json;
using QyWeixin.EF;

namespace QyWeixin
{
    /// <summary>
    /// 发货 的摘要说明
    /// </summary>
    public class 发货 : IHttpHandler
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
            switch(context.Request["行为"])
            {
                case "往来单位":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var set = from p in pinhua.往来单位 orderby p.RANK descending
                                  select new
                                  {
                                      p.单位编号,
                                      p.单位名称,
                                      p.单位地址,
                                      p.电话,
                                      p.传真,
                                      p.RANK,
                                      p.ExcelServerRCID,
                                  };
                        var json = JsonConvert.SerializeObject(set);
                        context.Response.Write(json);
                    }
                    break;
                case "单据流水":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var 每页行数 = int.Parse(context.Request["每页行数"]);
                        var a = (from p in pinhua.发货 orderby p.送货日期 descending, p.送货单号 descending
                                   select p).Take(每页行数);
                        var b = from p in pinhua.发货_DETAIL
                                group p by p.ExcelServerRCID into g
                                select new
                                {
                                    rcid = g.Key,
                                    total = g.Sum(x => x.金额),
                                    square = g.Sum(x => x.单位数量)
                                };
                        var set = from p1 in a.AsEnumerable()
                                  join p2 in b.AsEnumerable()
                                  on p1.ExcelServerRCID equals p2.rcid
                                  select new { p1.送货单号, p1.送货日期, p1.客户, p1.ExcelServerRCID, p1.业务描述, 金额 = (p2.total.HasValue ? p2.total.Value : 0).ToString("F2") + " 元" };
                        var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" };
                        var json = JsonConvert.SerializeObject(set, timeConverter);
                        context.Response.Write(json);
                    }
                    break;
                case "单据流水_单据明细":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var rcid = context.Request["rcid"];
                        var set = from p in pinhua.发货_DETAIL where p.ExcelServerRCID == rcid select p;
                        var json = JsonConvert.SerializeObject(set);
                        context.Response.Write(json);
                    }
                    break;
                case "发货明细":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var id = context.Request["单位编号"];
                        var rowstart = int.Parse(context.Request["行号"]);
                        var count = (from p in pinhua.发货 where p.客户编号 == id select p).Count();

                        
                            //var seta = pinhua.发货.ToList().Where(p => p.客户编号 == id).Select((p, rn) => new { rn, p });
                            var a = (from p in pinhua.发货
                                        where p.客户编号 == id
                                        orderby p.送货日期 descending, p.送货单号 descending
                                        select p).AsEnumerable().Select((p, rn) => new
                                        {
                                            rn,
                                            p
                                        });
                            var b = from p in pinhua.发货_DETAIL
                                        group p by p.ExcelServerRCID into g
                                        select new
                                        {
                                            rcid = g.Key,
                                            total = g.Sum(x => x.金额),
                                            square = g.Sum(x => x.单位数量)
                                        };
                            var ab = from p1 in a
                                     join p2 in b on p1.p.ExcelServerRCID equals p2.rcid
                                     orderby p1.p.送货日期 descending, p1.p.送货单号 descending
                                     select new { p1, p2 };


                            var set1 = from p1 in a.Take(rowstart + 10).Skip(rowstart)
                                   join p2 in b
                                       on p1.p.ExcelServerRCID equals p2.rcid
                                   orderby p1.p.送货日期 descending, p1.p.送货单号 descending
                                   select new
                                   {
                                       rn=p1.rn+1,
                                       p1.p.送货单号,
                                       p1.p.客户编号,
                                       p1.p.客户,
                                       p1.p.送货日期,
                                       p1.p.业务类型,
                                       p1.p.业务描述,
                                       p1.p.地址,
                                       p1.p.备注,
                                       p1.p.ExcelServerRCID,
                                       total = (p2.total.HasValue ? p2.total.Value : 0).ToString("F2") + " 元",
                                       square = (p2.square.HasValue ? p2.square.Value : 0).ToString("F2") + " ㎡"
                                   };
                        var set2 = from p in set1
                                   join d in pinhua.发货_DETAIL on p.ExcelServerRCID equals d.ExcelServerRCID
                                   select new
                                   {
                                       d.编号,
                                       d.描述,
                                       d.规格,
                                       d.PCS,
                                       d.计价单位,
                                       d.单位数量,
                                       d.单价,
                                       d.金额,
                                       d.木种,
                                       d.工艺,
                                       d.ExcelServerRCID
                                   };
                        var setfinal = new
                        {
                            总行数 = count,
                            单据信息 = set1,
                            发货明细 = set2
                        };
                        var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" };
                        var json = JsonConvert.SerializeObject(setfinal, timeConverter);
                        context.Response.Write(json);
                    }
                    break;
            }
        }
        private void ProcessPost(HttpContext context) 
        { 

        }
        public static String ToSBC(String input)
        {
            // 半角转全角：
            char[] c = input.ToCharArray();
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == 32)
                {
                    c[i] = (char)12288;
                    continue;
                }
                if (c[i] < 127)
                    c[i] = (char)(c[i] + 65248);
            }
            return new String(c);
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