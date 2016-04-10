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
        //
        // GET: /Sales/

        public ActionResult Index()
        {
            using (var pinhua = new PinhuaEntities())
            {
                var set = from p in pinhua.发货
                          select new Models.SalesModels.OrdersModel {
                              OrderId = p.送货单号,
                              CustomerId = p.客户编号,
                              CustomerName = p.客户
                          };

                return View(set.ToList());
            }
        }

    }
}
