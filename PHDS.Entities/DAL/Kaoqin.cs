using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL
{
    public static class Kaoqin
    {
        public static List<Edmx.考勤期间> 考勤区间()
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var listOf考勤期间 = from p in pinhua.考勤期间.AsNoTracking()
                                 orderby p.年 descending, p.月 descending
                                 select p;
                return listOf考勤期间.ToList();
            }
        }

        public static List<Edmx.考勤明细> 考勤明细(string RCID)
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var listOf考勤明细 = from p in pinhua.考勤明细.AsNoTracking()
                                 where p.ExcelServerRCID == RCID
                                 select p;
                return listOf考勤明细.ToList();
            }
        }
    }
}
