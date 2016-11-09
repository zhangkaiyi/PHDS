using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    static public class StockoutHelper
    {
        static public List<Models.StockoutModels.StockoutOrder> Orders
        {
            get
            {
                using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
                {
                    var set = from p in (from item in pinhua.发货_DETAIL.AsNoTracking()
                                         join itemprop in pinhua.物料登记.AsNoTracking() on item.编号 equals itemprop.编号
                                         select new { item, itemprop })
                              group p by p.item.ExcelServerRCID into g
                              let amount = g.Sum(x => x.item.金额 ?? 0)
                              let square = g.Sum(x => x.item.PCS * x.itemprop.Length * x.itemprop.Width / 1000 / 1000)
                              let count = g.Count(x => string.IsNullOrEmpty(x.item.ExcelServerRCID) == false)
                              select new { ExcelServerRCID = g.Key, Amount = amount, Square = square, Count = count };

                    var model = from p in pinhua.发货.AsNoTracking()
                                join p2 in set on p.ExcelServerRCID equals p2.ExcelServerRCID
                                join p3 in pinhua.业务类型.AsNoTracking() on p.业务类型 equals p3.业务类型1
                                //let contains = new string[] {"171","172"}
                                //where contains.Contains(p.业务类型)
                                where p3.MvP == "GI"
                                orderby p.送货日期 descending, p.送货单号 descending
                                select new Models.StockoutModels.StockoutOrder
                                {
                                    customerId = p.客户编号,
                                    customerName = p.客户,
                                    customerAddress = p.地址,
                                    orderId = p.送货单号,
                                    stockoutComment = p.备注,
                                    stockoutDate = p.送货日期,
                                    stockoutType = p.业务类型,
                                    stockoutTypeDescription = p3.类型描述,
                                    stockoutAmount = p2.Amount,
                                    stockoutSquare = p2.Square ?? 0,
                                    itemsCount = p2.Count,
                                    rcId = p.ExcelServerRCID
                                };
                    return model.ToList();
                }
            }
        }

        static public string rcId
        {
            get
            {
                using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
                {
                    var today = DateTime.UtcNow.ToString("yyyyMMdd");

                    var exsistedRcids = (from p in database.ES_RepCase
                                         where p.rcId.Substring(0, 10) == "rc" + today
                                         orderby p.rcId descending
                                         select p.rcId)
                                        .ToList();
                    var rcidIndex = int.Parse(exsistedRcids.Count() == 0 ? "0" : exsistedRcids.First().Substring(12)) + 1;
                    return "rc" + DateTime.UtcNow.ToString("yyyyMMdd") + rcidIndex.ToString("D5");
                }
            }
        }
        static public string rtId { get; } = "85.1";
    }
    [Authorize]
    public class StockoutController : BaseController
    {
        // GET: Stockout
        public ActionResult Index()
        {
            return View(StockoutHelper.Orders);
        }

        public ActionResult FahuoDetail(string RCID)
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var set = from p in pinhua.发货_DETAIL.AsNoTracking().AsEnumerable()
                          where p.ExcelServerRCID == RCID
                          orderby p.ExcelServerRN ascending
                          select new Models.SalesModels.OrderDetailModel
                          {
                              Id = p.编号,
                              Description = p.描述,
                              Size = p.规格,
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
                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
                jsonNetResult.Data = set.ToList();
                return jsonNetResult;
            }
        }

        public ActionResult Create()
        {
            return View();
        }
        public class CreateModel
        {
            public string rcId { get; set; }
            public string orderId { get; set; }
            public string 业务类型 { get; set; }
            public string 客户单位 { get; set; }
            public string 出库日期 { get; set; }
            public string 地址 { get; set; }
            public string 备注 { get; set; }
            public List<ItemModel> Items { get; set; }
        }
        public class ItemModel
        {
            public string 编号 { get; set; }
            public string 描述 { get; set; }
            public string 规格 { get; set; }
        }
        [HttpPost]
        public ActionResult Create(CreateModel model)
        {
            var rtId = "85.1";
            var repCase = new PHDS.Entities.Edmx.ES_RepCase
            {
                rcId = model.rcId,
                RtId = StockoutHelper.rtId,
                lstFiller = 2,
                lstFillerName = User.Identity.Name,
                lstFillDate = DateTime.UtcNow,
                //fillDate = DateTime.Now,
                //wiId = "",
                //state = 1,
            };
            var order = new PHDS.Entities.Edmx.发货
            {
                ExcelServerRCID = model.rcId,
                ExcelServerRTID = rtId,
                客户编号 = model.客户单位,
                业务类型 = model.业务类型,
                送货单号 = model.orderId,
                地址 = model.地址,
                备注 = model.备注,
                送货日期 = DateTime.Parse(model.出库日期),
            };

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                database.ES_RepCase.Add(repCase);
                database.发货.Add(order);
                if (model.Items?.Count() > 0)
                {
                    foreach (var item in model.Items)
                    {
                        database.发货_DETAIL.Add(new Entities.Edmx.发货_DETAIL
                        {
                            ExcelServerRCID = model.rcId,
                            ExcelServerRTID = rtId,
                            编号 = item.编号,
                            描述 = item.描述,
                            规格 = item.规格
                        });
                    }
                }
                database.SaveChanges();
            }

            return View("Index", StockoutHelper.Orders);
        }
    }
}