using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PHDS.Identity.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    [Authorize(Roles = "对账员")]
    public class 对账员Controller : Controller
    {
        #region Identity
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationRoleManager _roleManager;

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

        public ApplicationRoleManager RoleManager
        {
            get
            {
                return _roleManager ?? HttpContext.GetOwinContext().Get<ApplicationRoleManager>();
            }
            private set
            {
                _roleManager = value;
            }
        }
        #endregion

        public string[] Affiliations
        {
            get
            {
                return (UserManager.FindByName(User.Identity.Name).Affiliation ?? string.Empty).Split(',');
            }
        }

        public ActionResult Receivables()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                where Affiliations.Contains(p.单位编号)
                                orderby p.RANK descending, p.单位编号 ascending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("Statement", customers.ToList());
            }
        }

        public ActionResult Payables()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                where Affiliations.Contains(p.单位编号)
                                orderby p.RANK descending, p.单位编号 ascending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("Statement", customers.ToList());
            }
        }

        public ActionResult Both()
        {
            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var customers = from p in pinhua.往来单位
                                where Affiliations.Contains(p.单位编号)
                                orderby p.RANK descending, p.单位编号 ascending
                                select new Models.SalesModels.Customer
                                {
                                    Rank = p.RANK ?? 0,
                                    Id = p.单位编号,
                                    Name = p.单位名称
                                };
                return View("Statement", customers.ToList());
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

        public ActionResult YingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应付及明细(Id);
            return jsonNetResult;
        }

        public ActionResult YingshouYingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应收应付及明细(Id);
            return jsonNetResult;
        }
    }
}