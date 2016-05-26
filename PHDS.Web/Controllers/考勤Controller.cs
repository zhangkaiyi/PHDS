using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class 考勤Controller : Controller
    {
        // GET: 考勤
        public ActionResult Index()
        {
            var listOf考勤期间 = Entities.DAL.Kaoqin.考勤区间();
            return View(listOf考勤期间);
        }

        // GET: 考勤/Details/5
        public ActionResult Details(string RCID)
        {
            var jsonNetResult = new JsonNetResult();
            jsonNetResult.Formatting = Newtonsoft.Json.Formatting.Indented;
            jsonNetResult.SerializerSettings.DateFormatString = "yyyy/MM/dd";
            jsonNetResult.Data = Entities.DAL.Kaoqin.考勤明细(RCID);
            return jsonNetResult;
        }

        // GET: 考勤/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: 考勤/Create
        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: 考勤/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: 考勤/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: 考勤/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: 考勤/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
