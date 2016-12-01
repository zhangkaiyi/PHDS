using PHDS.Web.Models.StockoutModels;
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
                              let amount = g.Sum(x => x.item.金额)
                              let square = g.Sum(x => x.item.PCS * x.itemprop.Length * x.itemprop.Width / 1000 / 1000)
                              let count = g.Count(x => string.IsNullOrEmpty(x.item.ExcelServerRCID) == false)
                              select new { ExcelServerRCID = g.Key, Amount = amount, Square = square, Count = count };

                    var model = from p in pinhua.发货.AsNoTracking()
                                join p2 in set on p.ExcelServerRCID equals p2.ExcelServerRCID into leftjion
                                from pleft in leftjion.DefaultIfEmpty()
                                join p3 in pinhua.业务类型.AsNoTracking() on p.业务类型 equals p3.业务类型1
                                //let contains = new string[] {"171","172"}
                                //where contains.Contains(p.业务类型)
                                where p3.MvP == "GI"
                                orderby p.送货日期 descending, p.ExcelServerRCID descending
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
                                    stockoutAmount = pleft.Amount ?? 0,
                                    stockoutSquare = pleft.Square ?? 0,
                                    itemsCount = (int?)pleft.Count ?? 0,
                                    rcId = p.ExcelServerRCID
                                };
                    return model.ToList();
                }
            }
        }
        static public string OrderId
        {
            get
            {
                using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
                {
                    var year = DateTime.Now.ToString("yy");
                    var today = DateTime.Now.ToString("yyyyMMdd");

                    var exsistedIds = (from p in database.发货
                                       where p.送货单号.Substring(0, 5) == "OUT" + year
                                       orderby p.送货单号 descending
                                       select p.送货单号)
                                     .ToList();
                    //.Where(x => x.Substring(3, 4) == DateTime.Now.ToString("yyyy"));
                    var orderIndex = int.Parse(exsistedIds.Count() == 0 ? "0" : exsistedIds.First().Substring(7)) + 1;
                    return "OUT" + year + orderIndex.ToString("D5");
                }
            }
        }

        static public string rcId
        {
            get
            {
                using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
                {
                    var today = DateTime.Now.ToString("yyyyMMdd");

                    var exsistedRcids = (from p in database.ES_RepCase
                                         where p.rcId.Substring(0, 10) == "rc" + today
                                         orderby p.rcId descending
                                         select p.rcId)
                                        .ToList();
                    var rcidIndex = int.Parse(exsistedRcids.Count() == 0 ? "0" : exsistedRcids.First().Substring(12)) + 1;
                    return "rc" + today + rcidIndex.ToString("D5");
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

        [HttpPost]
        public ActionResult Index(string RCID)
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
            var model = new CreateModel
            {
                rcId = StockoutHelper.rcId,
                orderId = StockoutHelper.OrderId,
                stockoutDate = DateTime.UtcNow.ToString("yyyy-MM-dd")
            };
            return View(model);
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

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var order = new PHDS.Entities.Edmx.发货
                {
                    ExcelServerRCID = model.rcId,
                    ExcelServerRTID = rtId,
                    客户编号 = model.customerId,
                    客户 = database.往来单位.Find(model.customerId).单位名称,
                    业务类型 = model.stockoutType,
                    业务描述 = database.业务类型.Find(model.stockoutType).类型描述,
                    送货单号 = model.orderId,
                    地址 = model.stockoutAddress,
                    备注 = model.stockoutRemarks,
                    联系人 = model.stockoutContact,
                    联系电话 = model.stockoutContactNumber,
                    送货日期 = DateTime.Parse(model.stockoutDate),
                };

                database.ES_RepCase.Add(repCase);
                database.发货.Add(order);
                if (model.stockoutItems?.Count() > 0)
                {
                    foreach (var item in model.stockoutItems)
                    {
                        database.发货_DETAIL.Add(new Entities.Edmx.发货_DETAIL
                        {
                            ExcelServerRCID = model.rcId,
                            ExcelServerRN = item.rN,
                            ExcelServerRTID = rtId,
                            编号 = item.编号,
                            描述 = item.描述,
                            规格 = item.规格,
                            PCS = item.个数,
                            单位数量 = item.数量,
                            计价单位 = item.单位,
                            单价 = item.单价,
                            金额 = item.金额
                        });
                    }
                }
                database.SaveChanges();
            }

            return RedirectToAction("Index", StockoutHelper.Orders);
        }

        public ActionResult Edit(string Id)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var entity = database.发货.FirstOrDefault(x => x.送货单号 == Id);
                if (entity == null)
                    return RedirectToAction("Index");
                var items = database.发货_DETAIL.Where(x => x.ExcelServerRCID == entity.ExcelServerRCID).ToList();
                var itemList = new List<ItemModel>();
                items.ForEach(x => itemList.Add(new ItemModel
                {
                    rN = x.ExcelServerRN,
                    编号 = x.编号,
                    描述 = x.描述,
                    规格 = x.规格,
                    个数 = x.PCS,
                    数量 = x.单位数量,
                    单位 = x.计价单位,
                    单价 = x.单价,
                    金额 = x.金额,
                    Length = database.物料登记.FirstOrDefault(y => y.编号 == x.编号).Length,
                    Width = database.物料登记.FirstOrDefault(y => y.编号 == x.编号).Width,
                    Height = database.物料登记.FirstOrDefault(y => y.编号 == x.编号).Height
                }));
                var model = new CreateModel
                {
                    orderId = Id,
                    rcId = entity.ExcelServerRCID,
                    customerId = entity.客户编号,
                    customerName = entity.客户,
                    stockoutAddress = entity.地址,
                    stockoutContact = entity.联系人,
                    stockoutContactNumber = entity.联系电话,
                    stockoutDate = entity.送货日期?.ToString("yyyy-MM-dd"),
                    stockoutRemarks = entity.备注,
                    stockoutType = entity.业务类型,
                    //stockoutTypeDescription =,
                    stockoutItems = itemList
                };
                return View(model);
            }
        }

        [HttpPost, ActionName("Edit")]
        public ActionResult EditConfirmed(CreateModel model)
        {
            var rtId = "85.1";

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var orders = database.发货.Where(x => x.ExcelServerRCID == model.rcId);
                if (orders.Count() != 1)
                    return RedirectToAction("Index");

                var order = orders.First();
                order.客户编号 = model.customerId;
                order.送货日期 = DateTime.Parse(model.stockoutDate);
                order.业务类型 = model.stockoutType;
                order.地址 = model.stockoutAddress;
                order.备注 = model.stockoutRemarks;
                order.联系人 = model.stockoutContact;
                order.联系电话 = model.stockoutContactNumber;

                if (model.stockoutItems?.Count > 0)
                {
                    var items = database.发货_DETAIL.Where(x => x.ExcelServerRCID == model.rcId);
                    if (model.stockoutItems?.Count() > 0)
                        database.发货_DETAIL.RemoveRange(items);

                    foreach (var item in model.stockoutItems)
                    {
                        database.发货_DETAIL.Add(new Entities.Edmx.发货_DETAIL
                        {
                            ExcelServerRCID = model.rcId,
                            ExcelServerRTID = rtId,
                            ExcelServerRN = item.rN,
                            编号 = item.编号,
                            描述 = item.描述,
                            规格 = item.规格,
                            PCS = item.个数,
                            单位数量 = item.数量,
                            计价单位 = item.单位,
                            单价 = item.单价,
                            金额 = item.金额
                        });
                    }
                }
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(string Id)
        {
            var model = new CreateModel
            {
                orderId = Id,
            };
            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(CreateModel model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var rcId = database.发货.FirstOrDefault(x => x.送货单号 == model.orderId).ExcelServerRCID;
                var a = database.ES_RepCase.Where(x => x.rcId == rcId);
                if (a != null)
                    database.ES_RepCase.RemoveRange(a);
                var b = database.发货.Where(x => x.ExcelServerRCID == rcId);
                if (b != null)
                    database.发货.RemoveRange(b);
                var c = database.发货_DETAIL.Where(x => x.ExcelServerRCID == rcId);
                if (c != null)
                    database.发货_DETAIL.RemoveRange(c);
                database.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}