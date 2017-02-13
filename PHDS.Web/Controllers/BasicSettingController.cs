using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class BasicSettingController : BaseController
    {
        public ActionResult Wuliao()
        {
            return View();
        }

        public ActionResult WuliaoAdd()
        {
            var prefix = "WL";
            var length = 6;
            var model = new PHDS.Entities.Edmx.物料登记
            {
                编号 = Helpers.Pinhua.GenerateTrackingNo<Entities.Edmx.物料登记>(x => x.编号, prefix, length)

            };

            return View(model);
        }

        [HttpPost]
        public ActionResult WuliaoAdd(PHDS.Entities.Edmx.物料登记 model)
        {
            Helpers.Pinhua.NewRecord("物料登记", model);
            return RedirectToAction(Helpers.Common.GetActionName<BasicSettingController>(x => x.Wuliao()));
        }

        public ActionResult WuliaoEdit(string trackingNo)
        {
            var redirect = Helpers.Common.GetActionName<BasicSettingController>(x => x.Wuliao());
            if (string.IsNullOrEmpty(trackingNo))
                return RedirectToAction(redirect);

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var model = database.物料登记.AsNoTracking().FirstOrDefault(x => x.编号 == trackingNo);
                return View(model);
            }
        }

        [HttpPost]
        public ActionResult WuliaoEdit(PHDS.Entities.Edmx.物料登记 model)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var order = database.物料登记.FirstOrDefault(x => x.编号 == model.编号);
                Helpers.Pinhua.Copy.ShadowCopy(model, order);
                database.SaveChanges();

                //return RedirectToAction(Helpers.Common.GetActionName<BasicSettingController>(x => x.Wuliao()));
                return RedirectToAction(nameof(this.Wuliao));
            }
        }

        [HttpPost]
        public ActionResult WuliaoDelete(string rcId)
        {
            var redirect = Helpers.Common.GetActionName<BasicSettingController>(x => x.Wuliao());

            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var a = database.ES_RepCase.Where(x => x.rcId == rcId);
                if (a != null)
                    database.ES_RepCase.RemoveRange(a);
                var b = database.物料登记.Where(x => x.ExcelServerRCID == rcId);
                if (b != null)
                    database.物料登记.RemoveRange(b);
                database.SaveChanges();
            }

            return RedirectToAction(redirect);
        }
    }
}