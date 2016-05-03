using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Fahuo()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var amounts = from p in pinhua.发货_DETAIL.AsNoTracking()
                              group p by p.ExcelServerRCID into g
                              let sum = g.Sum(x => x.金额 ?? 0)
                              select new { ExcelServerRCID = g.Key, Amount = sum };

                var model = from p in pinhua.发货.AsNoTracking()
                            join pd in amounts on p.ExcelServerRCID equals pd.ExcelServerRCID
                            orderby p.送货日期 descending, p.送货单号 ascending
                            select new Models.SalesModels.OrdersModel
                            {
                                CustomerId = p.客户编号,
                                CustomerName = p.客户,
                                CustomerAddress = p.地址,
                                OrderId = p.送货单号,
                                SalesComment = p.备注,
                                SalesDate = p.送货日期,
                                SalesTypeId = p.业务类型,
                                SalesTypeDescription = p.业务描述,
                                SalesAmount = pd.Amount,
                                RCID = p.ExcelServerRCID
                            };

                return View(model.ToList());
            }
        }

        public string FahuoDetail(string RCID)
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var set = from p in pinhua.发货_DETAIL.AsNoTracking().AsEnumerable() where p.ExcelServerRCID == RCID
                          select new Models.SalesModels.OrderDetailModel
                          {
                              Id = p.编号,
                              Description = p.描述,
                              规格 = p.规格,
                              PCS = (p.PCS ?? 0).ToString("0.00"),
                              ChargeUnit = p.计价单位,
                              Price = (p.单价 ?? 0).ToString("0.00"),
                              UnitQuantity = (p.单位数量 ?? 0).ToString("0.00"),
                              Amount = (p.金额 ?? 0).ToString("0.00"),
                              Processing = p.工艺,
                              WoodSpecies = p.木种,
                              RCID = p.ExcelServerRCID,
                              RN = p.ExcelServerRN
                          };
                var ordersdetails = Newtonsoft.Json.JsonConvert.SerializeObject(set, new Newtonsoft.Json.Converters.IsoDateTimeConverter { DateTimeFormat = "yyyy/MM/dd" });

                return ordersdetails;
            }
        }

        public ActionResult Shouhuo()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}