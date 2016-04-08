using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Diagnostics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                                    count = g.Count(),
                                    total = g.Sum(x => x.金额),
                                    square = g.Sum(x => x.单位数量)
                                };
                        var set = from p1 in a.AsEnumerable()
                                  join p2 in b.AsEnumerable()
                                  on p1.ExcelServerRCID equals p2.rcid
                                  select new { p1.送货单号, p1.送货日期, p1.客户, p1.ExcelServerRCID, p1.业务描述, 金额 = (p2.total.HasValue ? p2.total.Value : 0).ToString("F2") + " 元", 项目数 = "共" + ToSBC(p2.count.ToString()) + "条记录" };
                        var timeConverter = new Newtonsoft.Json.Converters.IsoDateTimeConverter() { DateTimeFormat = "yyyy-MM-dd" };
                        var json = JsonConvert.SerializeObject(set, timeConverter);
                        context.Response.Write(json);
                    }
                    break;
                case "单据流水_单据明细":
                    using (var pinhua = new PinhuaEntities())
                    {
                        var rcid = context.Request["rcid"];
                        var set = from p in pinhua.发货_DETAIL where p.ExcelServerRCID == rcid orderby p.ExcelServerRN select p;
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
                case "物料清单":
                    using(var pinhua = new PinhuaEntities())
                    {
                        var set = from p in pinhua.物料登记
                                  select new { 
                                      p.编号,
                                      p.描述,
                                      p.规格,
                                      p.类型,
                                      p.Length,
                                      p.Width,
                                      p.Height
                                  };
                        var json = JsonConvert.SerializeObject(set);
                        context.Response.Write(json);
                    }
                    break;
            }
        }
        private void ProcessPost(HttpContext context) 
        { 
            switch(context.Request["行为"])
            {
                case "提交送货单":
                    using (var pinhua = new PinhuaEntities())
                    {


                        byte[] postbytes = new byte[context.Request.InputStream.Length];
                        context.Request.InputStream.Read(postbytes, 0, (Int32)context.Request.InputStream.Length);
                        var requestData = Encoding.UTF8.GetString(postbytes);
                        Debug.WriteLine(requestData);
                        var jobj = JsonConvert.DeserializeObject<SaveModel>(requestData);
                        
                        var rcid = "wx" + DateTime.Now.ToString("yyyyMMdd");
                        var rtid = "85.1";

                        var xxx = from p in pinhua.ES_RepCase
                                  where p.rcId.Substring(0, 10) == rcid
                                  orderby p.rcId.Substring(10) descending
                                  select p;

                        if (xxx.Count() == 0)
                        {
                            rcid += "00001";
                        }
                        else
                        {
                            var yyy = (int.Parse(string.IsNullOrEmpty(xxx.FirstOrDefault().rcId.Substring(10)) ? "0" : xxx.FirstOrDefault().rcId.Substring(10)) + 1).ToString("D5");
                            rcid += yyy;
                        }

                        var data_RepCase = new ES_RepCase
                        {
                            rcId = rcid,
                            RtId = rtid,
                            lstFiller = 2,
                            lstFillerName = "张凯译",
                            lstFillDate = DateTime.UtcNow,
                            //fillDate = DateTime.Now,
                            //wiId = "",
                            //state = 1,
                        };

                        var data_发货 = new QyWeixin.EF.发货
                        {
                            ExcelServerRCID = rcid,
                            ExcelServerRTID = rtid,
                            送货单号 = rcid,
                            客户编号 = jobj.客户编号,
                            客户 = jobj.客户名称,
                            地址 = jobj.地址,
                            送货日期 = jobj.日期,
                        };

                        var data_发货明细 = new List<发货_DETAIL>();
                        foreach (var item in jobj.发货明细)
                        {
                            var data = new 发货_DETAIL
                            {
                                ExcelServerRCID = rcid,
                                ExcelServerRTID = rtid,
                                ExcelServerRN = jobj.发货明细.IndexOf(item) + 1,
                                编号 = item.物料编号,
                                描述 = item.物料名称,
                                规格 = item.规格,
                                PCS = item.片数,
                                单位数量 = item.单位数量,
                                计价单位 = item.计价单位,
                                单价 = item.单价,
                                金额 = item.金额
                            };
                            data_发货明细.Add(data);
                        }
                        pinhua.ES_RepCase.Add(data_RepCase);
                        pinhua.发货.Add(data_发货);
                        foreach (var item in data_发货明细)
                        {
                            pinhua.发货_DETAIL.Add(item);
                        }
                        var count = pinhua.SaveChanges();
                        Debug.WriteLine(count);
                    }
                    break;
            }
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

    public class SaveModel
    {
        [JsonProperty(PropertyName = "id")]
        public string 客户编号 { get; set; }
        [JsonProperty(PropertyName = "name")]
        public string 客户名称 { get; set; }
        [JsonProperty(PropertyName = "address")]
        public string 地址 { get; set; }
        [JsonProperty(PropertyName = "thisdate")]
        public DateTime 日期 { get; set; }
        [JsonProperty(PropertyName = "comment")]
        public string 备注 { get; set; }
        [JsonProperty(PropertyName = "details")]
        public List<SaveDetailsModel> 发货明细 { get; set; }
    }
    public class SaveDetailsModel
    {
        [JsonProperty(PropertyName = "index")]
        public int 序号 { get; set; }
        [JsonProperty(PropertyName = "recordid")]
        public string 物料编号 { get; set; }
        [JsonProperty(PropertyName = "recordname")]
        public string 物料名称 { get; set; }
        [JsonProperty(PropertyName = "recordguige")]
        public string 规格 { get; set; }
        [JsonProperty(PropertyName = "recordpianshu")]
        public int 片数 { get; set; }
        [JsonProperty(PropertyName = "recorddanweishuliang")]
        public decimal 单位数量 { get; set; }
        [JsonProperty(PropertyName = "recordjijiadanwei")]
        public string 计价单位 { get; set; }
        [JsonProperty(PropertyName = "recorddanjia")]
        public decimal 单价 { get; set; }
        [JsonProperty(PropertyName = "recordjine")]
        public decimal 金额 { get; set; }
    }
}