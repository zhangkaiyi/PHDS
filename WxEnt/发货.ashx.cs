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
                case "发货清单":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var set = from p in pinhua.发货
                                   select p;
                    }
                    break;
                case "发货明细":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var id = context.Request["单位编号"];
                        var set1 = from p1 in
                                       (from p in pinhua.发货
                                        where p.客户编号 == id
                                        orderby p.送货日期 descending, p.送货单号 descending
                                        select p).Take(10)
                                   join p2 in
                                       (from p in pinhua.发货_DETAIL
                                        group p by p.ExcelServerRCID into g
                                        select new
                                        {
                                            rcid = g.Key,
                                            total = g.Sum(x => x.金额),
                                            square = g.Sum(x=>x.单位数量)
                                        }) on p1.ExcelServerRCID equals p2.rcid
                                   orderby p1.送货日期 descending, p1.送货单号 descending
                                   select new
                                   {
                                       p1.送货单号,
                                       p1.客户编号,
                                       p1.客户,
                                       p1.送货日期,
                                       p1.业务类型,
                                       p1.业务描述,
                                       p1.地址,
                                       p1.备注,
                                       p1.ExcelServerRCID,
                                       total = System.Data.Entity.SqlServer.SqlFunctions.StringConvert(p2.total.HasValue?p2.total.Value:0,10,2)+" 元",
                                       square=System.Data.Entity.SqlServer.SqlFunctions.StringConvert(p2.square.HasValue?p2.square.Value:0,10,2)+" ㎡"
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
                        //var withRN = set2.AsEnumerable().Select((item,index) => new { RN=index+1,item}); // 带上编号
                        var setfinal = new
                        {
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

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}