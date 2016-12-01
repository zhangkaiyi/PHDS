using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class MaterialController : Controller
    {
        // GET: Material
        public ActionResult Index()
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var materials = database.物料登记.ToList();
                return View(materials);
            }
        }
    }
}