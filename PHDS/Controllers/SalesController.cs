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
                var set = from p in pinhua.发货
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

    }
}
