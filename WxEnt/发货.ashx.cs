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
                        var set = from p in pinhua.发货_DETAIL
                                   select p;
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