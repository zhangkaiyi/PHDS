using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class CaiwuController : BaseController
    {
        public class RanksModel
        {
            public int RankStart { get; set; }
            public int RankEnd { get; set; }
            public string Title { get; set; }
        }
        // GET: Caiwu
        public ActionResult ChaZhang(string partnerId)
        {
            var result = Entities.DAL.应收应付.Api.应收应付及明细(partnerId);
            return View(result);
        }

        public ActionResult Shoukuan()
        {
            return View();
        }

        public ActionResult ShoukuanAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ShoukuanAdd(Entities.Edmx.收款单 model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var year = DateTime.Now.ToString("yy");

                var exsistedIds = (from p in database.收款单
                                   where p.收款单号.Substring(0, 4) == "SK" + year
                                   orderby p.收款单号 descending
                                   select p.收款单号)
                                 .ToList();
                var orderIndex = int.Parse(exsistedIds.Count() == 0 ? "0" : exsistedIds.First().Substring(4,6)) + 1;

                var name = database.往来单位.FirstOrDefault(x => x.单位编号 == model.单位编号)?.单位名称;

                model.收款单号 = "SK" + year + orderIndex.ToString("D6");
                model.收款单位 = name;

                Helpers.Pinhua.NewRecord("收款单", model);
            }

            return RedirectToAction("Shoukuan");
        }

        public ActionResult ShoukuanEdit(string orderNo)
        {
            if(string.IsNullOrEmpty(orderNo))
                return RedirectToAction("Shoukuan");

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var orders = from p in database.收款单
                             where p.收款单号 == orderNo
                             select p;

                return View(model: orders.Count() > 0 ? orders.First() : null);
            }
        }

        [HttpPost, ActionName("ShoukuanEdit")]
        public ActionResult ShoukuanEdit(Entities.Edmx.收款单 model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var order = database.收款单.FirstOrDefault(x => x.ExcelServerRCID == model.ExcelServerRCID);
                model.收款单位 = database.往来单位.FirstOrDefault(x => x.单位编号 == model.单位编号)?.单位名称;

                Helpers.Pinhua.Copy.ShadowCopy(model, order);

                database.SaveChanges();
            }

            return RedirectToAction("Shoukuan");
        }

        [HttpPost]
        public ActionResult ShoukuanDelete(string rcId)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var a = database.ES_RepCase.Where(x => x.rcId == rcId);
                if (a != null)
                    database.ES_RepCase.RemoveRange(a);
                var b = database.收款单.Where(x => x.ExcelServerRCID == rcId);
                if (b != null)
                    database.收款单.RemoveRange(b);
                database.SaveChanges();
            }

            return RedirectToAction("Shoukuan");
        }

        public ActionResult Fukuan()
        {
            return View();
        }

        public ActionResult FukuanAdd()
        {
            return View();
        }

        [HttpPost]
        public ActionResult FukuanAdd(Entities.Edmx.付款单 model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var year = DateTime.Now.ToString("yy");

                var exsistedIds = (from p in database.付款单
                                   where p.付款单号.Substring(0, 4) == "FK" + year
                                   orderby p.付款单号 descending
                                   select p.付款单号)
                                 .ToList();
                var orderIndex = int.Parse(exsistedIds.Count() == 0 ? "0" : exsistedIds.First().Substring(4, 6)) + 1;

                var name = database.往来单位.FirstOrDefault(x => x.单位编号 == model.单位编号)?.单位名称;

                model.付款单号 = "FK" + year + orderIndex.ToString("D6");
                model.付款单位 = name;

                Helpers.Pinhua.NewRecord("付款单", model);
            }

            return RedirectToAction("Fukuan");
        }

        public ActionResult FukuanEdit(string orderNo)
        {
            if (string.IsNullOrEmpty(orderNo))
                return RedirectToAction("Fukuan");

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var orders = from p in database.付款单
                             where p.付款单号 == orderNo
                             select p;

                return View(model: orders.Count() > 0 ? orders.First() : null);
            }
        }

        [HttpPost, ActionName("FukuanEdit")]
        public ActionResult FukuanEdit(Entities.Edmx.付款单 model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var order = database.付款单.FirstOrDefault(x => x.ExcelServerRCID == model.ExcelServerRCID);
                model.付款单位 = database.往来单位.FirstOrDefault(x => x.单位编号 == model.单位编号)?.单位名称;

                Helpers.Pinhua.Copy.ShadowCopy(model, order);

                database.SaveChanges();
            }

            return RedirectToAction("Fukuan");
        }

        [HttpPost]
        public ActionResult FukuanDelete(string rcId)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var a = database.ES_RepCase.Where(x => x.rcId == rcId);
                if (a != null)
                    database.ES_RepCase.RemoveRange(a);
                var b = database.付款单.Where(x => x.ExcelServerRCID == rcId);
                if (b != null)
                    database.付款单.RemoveRange(b);
                database.SaveChanges();
            }

            return RedirectToAction("Fukuan");
        }
    }
}