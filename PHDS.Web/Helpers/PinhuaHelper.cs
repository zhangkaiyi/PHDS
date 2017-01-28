using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PHDS.Web.Helpers
{
    public class PinhuaHelper
    {
        static public string CreateRcId()
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var today = DateTime.Now.ToString("yyyyMMdd");
                var id = database.GetNewId_s(26, 1).FirstOrDefault().newId.Value.ToString("D5");
                return "rc" + today + id;
            }
        }

        static public string GetRtId(string templateName)
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var rtId = from p in database.ES_Tmp
                           where p.RtName == templateName
                           select p.RtId;
                return rtId.First() ?? string.Empty;
            }
        }

        static public void NewRecord<M>(string tempateName, M record) where M : class
        {
            using (var database = new PHDS.Entities.Edmx.PinhuaEntities())
            {
                var rcId = CreateRcId();
                var rtId = GetRtId(tempateName);
                var repCase = new PHDS.Entities.Edmx.ES_RepCase
                {
                    rcId = rcId,
                    RtId = rtId,
                    lstFiller = 2,
                    lstFillerName = HttpContext.Current.User.Identity.Name,
                    lstFillDate = DateTime.UtcNow,
                    fillDate = DateTime.UtcNow,
                };
                if (typeof(M) == typeof(PHDS.Entities.Edmx.收款单))
                {
                    (record as PHDS.Entities.Edmx.收款单).ExcelServerRCID = rcId;
                    (record as PHDS.Entities.Edmx.收款单).ExcelServerRTID = rtId;
                    database.ES_RepCase.Add(repCase);
                    database.收款单.Add(record as PHDS.Entities.Edmx.收款单);
                    database.SaveChanges();
                }
                else if (typeof(M) == typeof(PHDS.Entities.Edmx.付款单))
                {
                    (record as PHDS.Entities.Edmx.付款单).ExcelServerRCID = rcId;
                    (record as PHDS.Entities.Edmx.付款单).ExcelServerRTID = rtId;
                    database.ES_RepCase.Add(repCase);
                    database.付款单.Add(record as PHDS.Entities.Edmx.付款单);
                    database.SaveChanges();
                }
            }
        }

        static public void NewRecord<M, D>(M main, D details)
        {
            foreach (var single in details as IEnumerable<int>)
            {

            }
        }

        static public void DeleteRecord(string tempateName, string RcId)
        {
            
        }

        public static class Copy
        {
            public static void ShadowCopy(object from, object to)
            {
                if (from.GetType() == to.GetType())
                {
                    System.Reflection.PropertyInfo[] properties = from.GetType().GetProperties();

                    foreach (var p in properties)
                    {
                        var value = p.GetValue(from, null);
                        if (value != GetDefault(p.GetType()))
                            p.SetValue(to, value, null);
                    }
                }
            }

            public static object GetDefault(Type type)
            {
                if (type.IsValueType)
                {
                    return Activator.CreateInstance(type);
                }
                return null;
            }
        }
    }
}