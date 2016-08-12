using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class TestController : BaseController
    {
        // GET: Test
        [Description("权限列表")]
        public ActionResult Index()
        {
            var permissions = _permissionsOfAssembly.ToList();
            
            return View(permissions);
        }
    }
}