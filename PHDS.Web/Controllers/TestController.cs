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
        [Permission("C863AEB6-77BD-4779-B477-B5D7EBE75EAF")]
        [Description("权限列表")]
        public ActionResult Index()
        {
            var permissions = _permissionsOfAssembly.ToList();
            
            return View(permissions);
        }
    }
}