﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    [Authorize]
    public class StockoutController : BaseController
    {
        // GET: Stockout
        public ActionResult Index()
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
    }
}