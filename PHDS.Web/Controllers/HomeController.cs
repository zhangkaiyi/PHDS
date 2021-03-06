﻿using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PHDS.Identity.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        //[AllowAnonymous]
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

                return View(model.ToList());
            }
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
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应收及明细(Id);
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
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应付及明细(Id);
            return jsonNetResult;
        }

        public ActionResult YingshouYingfu()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var bRole = UserManager.IsInRole(User.Identity.GetUserId(), "管理员") || UserManager.IsInRole(User.Identity.GetUserId(), "访客");
                if (bRole)
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
                else
                {
                    return View("YingshouYingfu", new List<Models.SalesModels.Customer>());
                }
            }
        }

        public ActionResult YingshouYingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应收应付及明细(Id);
            return jsonNetResult;
        }

        public ActionResult Zaitu()
        {

            return View(Entities.DAL.出库入库.Api.在途物料());
        }

        public ActionResult ZaituDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Id == "All" ? Entities.DAL.出库入库.Api.在途物料() : Entities.DAL.出库入库.Api.在途物料byId(Id);
            return jsonNetResult;

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