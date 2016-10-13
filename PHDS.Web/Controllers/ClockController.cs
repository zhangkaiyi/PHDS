using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class ClockController : BaseController
    {
        // GET: Clock
        public ActionResult Index()
        {
            return View();
        }
    }
}