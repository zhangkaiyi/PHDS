using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PHDS.Identity.BLL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    [IdentityAuthorize(Roles = "对账员")]
    public class 对账员Controller : BaseController
    {
        public string[] Affiliations
        {
            get
            {
                return (_userManager.FindByName(User.Identity.Name).Affiliation ?? string.Empty).Split(',');
            }
        }

        [Permission("E1126F3D-688A-459E-A84D-F9F9B04057E3")]
        [Description("对账员专用 - 应收款查询")]
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

        [Permission("41EEF1E2-8127-4D07-85CC-98594E49A9DD")]
        [Description("对账员专用 - 应付款查询")]
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

        [Permission("23126B5B-17C8-4E42-93C2-A16E5916D587")]
        [Description("对账员专用 - 应收应付联合查询")]
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

        [Permission("CDBA9DF2-11A4-4AA1-AB07-F22349C3C62B")]
        [Description("对账员专用 - 应收明细JSON")]
        public ActionResult YingshouDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应收及明细(Id);
            return jsonNetResult;
        }

        [Permission("CDD456C6-8B21-4774-A68B-8C0BFFA6639D")]
        [Description("对账员专用 - 应付明细JSON")]
        public ActionResult YingfuDetail(string Id)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.应收应付.Api.应付及明细(Id);
            return jsonNetResult;
        }

        [Permission("6834D32F-C838-4278-94C4-68A8BC399AB2")]
        [Description("对账员专用 - 应收应付联合明细JSON")]
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