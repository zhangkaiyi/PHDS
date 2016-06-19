using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL.出库入库
{
    public class Api
    {
        static public List<Models.在途物料> 在途物料()
        {
            using (var pinhua = new Entities.Edmx.PinhuaEntities())
            {
                var waixie = from p1 in pinhua.发货.AsNoTracking()
                             join p2 in pinhua.发货_DETAIL.AsNoTracking() on p1.ExcelServerRCID equals p2.ExcelServerRCID
                             join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                             where p1.业务类型.Contains("74") || p1.业务类型 == "171"
                             select new
                             {
                                 p1.客户编号,
                                 p2.编号,
                                 数量 = p2.PCS * p3.在途计算
                             };
                var waixieGroupById = from p in waixie
                                      group p by new { p.客户编号, p.编号 } into g
                                      orderby g.Key.客户编号 ascending
                                      select new
                                      {
                                          CustomerId = g.Key.客户编号,
                                          ItemId = g.Key.编号,
                                          Count = g.Sum(e => e.数量)
                                      };

                var groupWithDetails = from p1 in waixieGroupById
                                       join p2 in pinhua.往来单位.AsNoTracking()
                                       on p1.CustomerId equals p2.单位编号
                                       join p3 in pinhua.物料登记.AsNoTracking()
                                       on p1.ItemId equals p3.编号
                                       where p1.Count != 0
                                       orderby p2.RANK descending, p1.CustomerId
                                       select new Entities.DAL.出库入库.Models.在途物料
                                       {
                                           单位编号 = p1.CustomerId,
                                           单位名称 = p2.单位名称,
                                           产品编号 = p1.ItemId,
                                           产品描述 = p3.描述,
                                           规格 = p3.规格,
                                           数量 = p1.Count ?? 0,
                                       };

                return groupWithDetails.ToList();
            }
        }
        static public List<Models.在途物料> 在途物料byId(string Id)
        {
            var zaitu = from p in 在途物料()
                        where p.单位编号 == Id
                        select p;
            return zaitu.ToList();
        }
    }
}
