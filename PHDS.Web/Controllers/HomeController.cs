using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class 结构体
    {
        public class 在途
        {
            public string 单位编号 { get; set; }
            public string 单位名称 { get; set; }
            public string 产品编号 { get; set; }
            public string 产品描述 { get; set; }
            public string 规格 { get; set; }
            public decimal 数量 { get; set; }

        }
    }
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
                            select new Models.SalesModels.OrdersModel
                            {
                                CustomerId = p.客户编号,
                                CustomerName = p.客户,
                                CustomerAddress = p.地址,
                                OrderId = p.送货单号,
                                SalesComment = p.备注,
                                SalesDate = p.送货日期,
                                SalesTypeId = p.业务类型,
                                SalesTypeDescription = p3.类型描述,
                                SalesAmount = p2.Amount,
                                SalesSquare = p2.Square ?? 0,
                                DetailsCount = p2.Count,
                                RCID = p.ExcelServerRCID
                            };

                return View(model.ToList());
            }
        }

        public ActionResult Fahuo2()
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
                            //let contains = new string[]
                            //{
                            //    "601", "602", "603", "604", "611", "621", "741", "742"
                            //}
                            //where contains.Contains(p.业务类型)
                            where p3.MvP == "GI"
                            orderby p.送货日期 descending, p.送货单号 descending
                            select new Models.SalesModels.OrdersModel
                            {
                                CustomerId = p.客户编号,
                                CustomerName = p.客户,
                                CustomerAddress = p.地址,
                                OrderId = p.送货单号,
                                SalesComment = p.备注,
                                SalesDate = p.送货日期,
                                SalesTypeId = p.业务类型,
                                SalesTypeDescription = p3.类型描述,
                                SalesAmount = p2.Amount,
                                SalesSquare = p2.Square ?? 0,
                                DetailsCount = p2.Count,
                                RCID = p.ExcelServerRCID
                            };

                return View(model.ToList());
            }
        }

        public ActionResult FahuoDetail(string RCID)
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var set = from p in pinhua.发货_DETAIL.AsNoTracking().AsEnumerable() where p.ExcelServerRCID == RCID
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

        public ActionResult Shouhuo()
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
                            where p3.MvP == "GR"
                            orderby p.送货日期 descending, p.送货单号 descending
                            select new Models.SalesModels.OrdersModel
                            {
                                CustomerId = p.客户编号,
                                CustomerName = p.客户,
                                CustomerAddress = p.地址,
                                OrderId = p.送货单号,
                                SalesComment = p.备注,
                                SalesDate = p.送货日期,
                                SalesTypeId = p.业务类型,
                                SalesTypeDescription = p3.类型描述,
                                SalesAmount = p2.Amount,
                                SalesSquare = p2.Square ?? 0,
                                DetailsCount = p2.Count,
                                RCID = p.ExcelServerRCID
                            };

                return View(model.ToList());
            }
        }

        public ActionResult ShouhuoDetail(string RCID)
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

        public ActionResult Wanglai()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                orderby p.RANK descending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View(customers.ToList());
            }
        }

        public ActionResult WanglaiDetail(string Id)
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var details = from p1 in pinhua.发货
                                join p2 in pinhua.发货_DETAIL on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                join p3 in pinhua.业务类型 on p1.业务类型 equals p3.业务类型1
                                where p1.客户编号 == Id
                                orderby p1.送货日期 descending, p1.送货单号 descending
                                select new
                                {
                                    p1.送货单号,
                                    p1.送货日期,
                                    p1.业务类型,
                                    p1.业务描述,
                                    p2.编号,
                                    p2.描述,
                                    p2.PCS,
                                    p2.规格,
                                    p2.单位数量,
                                    p2.计价单位,
                                    p2.单价,
                                    p2.金额,
                                    p3.业务计算,
                                    p3.库存计算,
                                    p3.对账计算
                                };
                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
                jsonNetResult.Data = details.ToList();
                return jsonNetResult;
            }
        }

        public ActionResult Yingshou()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                orderby p.RANK descending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("YingshouYingfu", customers.ToList());
            }
        }

        public ActionResult YingshouDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.YingShouYingFu.应收及明细(Id);
            return jsonNetResult;
        }

        public ActionResult Yingfu()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                orderby p.RANK descending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("YingshouYingfu", customers.ToList());
            }
        }

        public ActionResult YingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.YingShouYingFu.应付及明细(Id);
            return jsonNetResult;
        }

        public ActionResult YingshouYingfu()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                orderby p.RANK descending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("YingshouYingfu", customers.ToList());
            }
        }

        public ActionResult YingshouYingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.YingShouYingFu.应收应付及明细(Id);
            return jsonNetResult;
        }

        public ActionResult Zaitu()
        {
            using (var pinhua = new Entities.Edmx.PinhuaEntities()) 
            {
                var song = from p1 in pinhua.发货.AsNoTracking()
                           join p2 in pinhua.发货_DETAIL.AsNoTracking()
                           on p1.ExcelServerRCID equals p2.ExcelServerRCID
                           join p3 in pinhua.业务类型.AsNoTracking()
                           on p1.业务类型 equals p3.业务类型1
                           where p1.业务类型.Contains("74") || p1.业务类型 == "171"
                           select new
                           {
                               p1.客户编号,
                               p2.编号,
                               数量 = p2.PCS * p3.在途计算
                           };
                var set1 = from p in song
                          group p by new { p.客户编号, p.编号 } into g
                          orderby g.Key.客户编号 ascending
                          select new
                          {
                              CustomerId = g.Key.客户编号,
                              ItemId = g.Key.编号,
                              Count = g.Sum(e => e.数量)
                          };

                var set = from p1 in set1
                          join p2 in pinhua.往来单位.AsNoTracking()
                          on p1.CustomerId equals p2.单位编号
                          join p3 in pinhua.物料登记.AsNoTracking()
                          on p1.ItemId equals p3.编号
                          where p1.Count != 0
                          orderby p2.RANK descending, p1.CustomerId
                          select new 结构体.在途
                          {
                              单位编号 = p1.CustomerId,
                              单位名称 = p2.单位名称,
                              产品编号 = p1.ItemId,
                              产品描述 = p3.描述,
                              规格 = p3.规格,
                              数量 = p1.Count ?? 0,
                          };

                return View(set.ToList());
            }
        }

        public ActionResult ZaituDetail(string Id)
        {
            using (var pinhua = new Entities.Edmx.PinhuaEntities())
            {
                var song = from p1 in pinhua.发货.AsNoTracking()
                           join p2 in pinhua.发货_DETAIL.AsNoTracking()
                           on p1.ExcelServerRCID equals p2.ExcelServerRCID
                           join p3 in pinhua.业务类型.AsNoTracking()
                           on p1.业务类型 equals p3.业务类型1
                           where p1.业务类型.Contains("74") || p1.业务类型 == "171"
                           select new
                           {
                               p1.客户编号,
                               p2.编号,
                               数量 = p2.PCS * p3.在途计算
                           };
                var set1 = from p in song
                           group p by new { p.客户编号, p.编号 } into g
                           orderby g.Key.客户编号 ascending
                           select new
                           {
                               CustomerId = g.Key.客户编号,
                               ItemId = g.Key.编号,
                               Count = g.Sum(e => e.数量)
                           };

                var set2 = from p1 in set1
                          join p2 in pinhua.往来单位.AsNoTracking()
                          on p1.CustomerId equals p2.单位编号
                          join p3 in pinhua.物料登记.AsNoTracking()
                          on p1.ItemId equals p3.编号
                          where p1.Count != 0
                          orderby p2.RANK descending, p1.CustomerId
                          select new 结构体.在途
                          {
                              单位编号 = p1.CustomerId,
                              单位名称 = p2.单位名称,
                              产品编号 = p1.ItemId,
                              产品描述 = p3.描述,
                              规格 = p3.规格,
                              数量 = p1.Count ?? 0,
                          };
                var set = new List<结构体.在途>();
                if (Id == "All")
                    set = set2.ToList();
                else
                    set = set2.Where(e => e.单位编号 == Id).ToList();

                var jsonNetResult = new JsonNetResult();
                jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
                jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
                jsonNetResult.Data = set.ToList();
                return jsonNetResult;
            }
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