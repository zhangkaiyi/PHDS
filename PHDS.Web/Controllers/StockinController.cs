using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    [Authorize]
    public class StockinController : BaseController
    {
        // GET: Stockin
        public ActionResult Index()
        {
            return View();
        }
    }
}