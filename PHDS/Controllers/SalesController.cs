using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PHDS.EF;

namespace PHDS.Controllers
{
    public class SalesController : Controller
    {
        public ActionResult Index()
        {
            using (var pinhua = new PinhuaEntities())
            {
                var set = from p in pinhua.发货.AsNoTracking()
                          select new Models.SalesModels.OrdersModel
                          {
                              RCID = p.ExcelServerRCID,
                              OrderId = p.送货单号,
                              CustomerId = p.客户编号,
                              CustomerName = p.客户,
                              SaleDate = p.送货日期.Value,
                              CustomerAddress = p.地址,
                              SalesTypeId = p.业务类型,
                              SalesTypeDescription = p.业务描述,
                          };

                return View(set.ToList());
            }
        }

        [HttpPost]
        public ActionResult 删除单据(string rcid)
        {
            using (var pinhua = new PinhuaEntities())
            {
                int delCount = 0;
                foreach (var p in (from p in pinhua.ES_RepCase where p.rcId == rcid select p))
                {
                    pinhua.ES_RepCase.Remove(p);
                    delCount++;
                }
                foreach (var p in (from p in pinhua.发货 where p.ExcelServerRCID == rcid select p))
                {
                    pinhua.发货.Remove(p);
                    delCount++;
                }
                foreach (var p in (from p in pinhua.发货_DETAIL where p.ExcelServerRCID == rcid select p))
                {
                    pinhua.发货_DETAIL.Remove(p);
                    delCount++;
                }
                using (var trans = pinhua.Database.BeginTransaction())
                {
                    var realCount = pinhua.SaveChanges();

                    if (realCount == delCount)
                        trans.Commit();
                    else
                        trans.Rollback();

                    return Content(realCount.ToString());
                }
            }
        }

    }
}
