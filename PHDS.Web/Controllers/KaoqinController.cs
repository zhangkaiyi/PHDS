using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PHDS.Web.Controllers
{
    public class KaoqinController : Controller
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
        
        public ActionResult TimeRecords()
        {
            var listOf考勤期间 = Entities.DAL.Kaoqin.考勤区间();
            return View(listOf考勤期间);
        }

        public ActionResult RecordDetails(string id, DateTime? time)
        { 
            if (string.IsNullOrEmpty(id) || !time.HasValue)
                return RedirectToAction("TimeRecords");

            using (var pinhua = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var card_ids = (from p1 in pinhua.人员档案.AsNoTracking()
                               join p2 in pinhua.考勤卡号变动 on p1.ExcelServerRCID equals p2.ExcelServerRCID
                               where p1.人员编号 == id
                               select p2.卡号).ToList();

                var manual_records = (from p1 in pinhua.打卡登记.AsNoTracking()
                                      where p1.时间.Value.Year == time.Value.Year
                                      && p1.时间.Value.Month == time.Value.Month
                                      && p1.时间.Value.Day == time.Value.Day
                                      && p1.人员编号 == id
                                      select p1).ToList();

                using (var eastriver = new PHDS.Entities.Edmx.EastRiverEntities())
                {
                    var records = (from p1 in eastriver.TimeRecords.AsNoTracking()
                                  where card_ids.Contains(p1.card_id)
                                  && time.Value.Year == p1.sign_time.Year
                                  && time.Value.Month == p1.sign_time.Month
                                  && time.Value.Day == p1.sign_time.Day
                                  select p1).ToList();
                    foreach(var item in manual_records)
                    {
                        records.Add(new PHDS.Entities.Edmx.TimeRecords { card_id = item.ExcelServerRCID, sign_time = item.时间.Value });
                    }

                    return View(records.ToList());
                }
            }
            //dynamic xxx = new System.Dynamic.ExpandoObject();
            //return View((object)xxx);
        }

        public ActionResult MonthReport(int year, int month, string rcid)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var r1 = from p1 in database.考勤明细
                         where p1.ExcelServerRCID == rcid
                         select p1;
                return View(r1.ToList());
            }
        }
    }
}
