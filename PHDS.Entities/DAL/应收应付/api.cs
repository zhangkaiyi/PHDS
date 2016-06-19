using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL.应收应付
{
    public class Api
    {
        private static IEnumerable<Edmx.对账结算_主表> listOf对账结算(string Id) // 对应Id的对账结算数据
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfDuizhang = from p in pinhua.对账结算_主表
                                        where p.单位编号 == Id
                                        select p;
                return detailsOfDuizhang.ToList();
            }
        }
        private static IEnumerable<Edmx.收款单> listOf收款(string Id) // 对应Id的收款单
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfShoukuan = from p in pinhua.收款单
                                        where p.单位编号 == Id
                                        select p;
                return detailsOfShoukuan.ToList();
            }
        }

        private static IEnumerable<Edmx.付款单> listOf付款(string Id) // 对应Id的收款单
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfFukuan = from p in pinhua.付款单
                                        where p.单位编号 == Id
                                        select p;
                return detailsOfFukuan.ToList();
            }
        }

        private static IEnumerable<Models.Model出库入库> listOf出库(string Id) // 对应Id的发货详情
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfFa = from p1 in pinhua.发货.AsNoTracking()
                                  join p2 in pinhua.发货_DETAIL.AsNoTracking() on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                  join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                  join p4 in pinhua.物料登记.AsNoTracking() on p2.编号 equals p4.编号
                                  where p1.客户编号 == Id && p3.MvP == "GI"
                                  select new Models.Model出库入库
                                  {
                                      Record = new Models.Model出库入库条目
                                      {
                                          客户编号 = p1.客户编号,
                                          单号 = p1.送货单号,
                                          送货日期 = p1.送货日期,
                                          业务类型 = p1.业务类型,
                                          业务描述 = p3.类型描述,
                                          备注 = p1.备注
                                      },
                                      RecordDetail = new Models.Model出库入库明细
                                      {
                                          编号 = p2.编号,
                                          描述 = p4.描述,
                                          规格 = p2.规格,
                                          片数 = p2.PCS,
                                          计价单位 = p2.计价单位,
                                          单位数量 = p2.单位数量,
                                          单价 = p2.单价,
                                          金额 = p2.金额 * p3.业务计算
                                      }
                                  };
                return detailsOfFa.ToList();
            }
        }

        private static IEnumerable<Models.Model出库入库> listOf入库(string Id) // 对应Id的收货详情
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfShou1 = from p1 in pinhua.发货.AsNoTracking()
                                     join p2 in pinhua.发货_DETAIL.AsNoTracking() on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                     join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                     join p4 in pinhua.物料登记.AsNoTracking() on p2.编号 equals p4.编号
                                     where p1.客户编号 == Id && p3.MvP == "GR"
                                     select new Models.Model出库入库
                                     {
                                         Record = new Models.Model出库入库条目
                                         {
                                             客户编号 = p1.客户编号,
                                             单号 = p1.送货单号,
                                             送货日期 = p1.送货日期,
                                             业务类型 = p1.业务类型,
                                             业务描述 = p3.类型描述,
                                             备注 = p1.备注
                                         },
                                         RecordDetail = new Models.Model出库入库明细
                                         {
                                             编号 = p2.编号,
                                             描述 = p4.描述,
                                             规格 = p2.规格,
                                             片数 = p2.PCS,
                                             计价单位 = p2.计价单位,
                                             单位数量 = p2.单位数量,
                                             单价 = p2.单价,
                                             金额 = p2.金额 * p3.业务计算
                                         }
                                     };
                var detailsOfShou2 = from p1 in pinhua.收货.AsNoTracking()
                                     join p2 in pinhua.收货_D.AsNoTracking()
                                     on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                     join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                     where p1.供应商编号 == Id && p3.MvP == "GR"
                                     select new Models.Model出库入库
                                     {
                                         Record = new Models.Model出库入库条目
                                         {
                                             客户编号 = p1.供应商编号,
                                             单号 = p1.单号,
                                             送货日期 = p1.日期,
                                             业务类型 = p1.业务类型,
                                             业务描述 = p3.类型描述,
                                             备注 = ""
                                         },
                                         RecordDetail = new Models.Model出库入库明细
                                         {
                                             编号 = p2.编号,
                                             描述 = p2.名称,
                                             规格 = p2.规格,
                                             片数 = p2.PCS,
                                             计价单位 = p2.计价单位,
                                             单位数量 = p2.单位数量,
                                             单价 = p2.单价,
                                             金额 = p2.金额
                                         }
                                     };

                var detailsOfShou = detailsOfShou1.ToList().Union(detailsOfShou2.ToList());
                return detailsOfShou;
            }
        }

        public static IEnumerable<Models.Model应收应付> 应收(string Id)
        {
            var lastDate = listOf对账结算(Id).Max(x => x.日期); // 最新对账日期
            var lastAmount = listOf对账结算(Id).Where(x => x.日期 == lastDate); // 最新对账金额

            // 对账后的收发详情
            var listOfFaAfter = listOf出库(Id).Where(x => x.Record.送货日期 > lastDate);
            var listOfShouAfter = listOf入库(Id).Where(x => x.Record.送货日期 > lastDate);
            // 对账后的收款详情
            var listOfShoukuanAfter = listOf收款(Id).Where(x => x.收款日期 > lastDate);

            var yingShou = from p in listOfFaAfter
                           let GI = from pp in listOfFaAfter
                                    where pp.Record.单号 == p.Record.单号
                                    select pp
                           select new Models.Model应收应付
                           {
                               应收合计 = GI.Sum(x => x.RecordDetail.金额),
                           };

            return yingShou;
        }
        public static IEnumerable<Models.Model应收应付> 应付(string Id)
        {
            var lastDate = listOf对账结算(Id).Max(x => x.日期); // 最新对账日期
            var lastAmount = listOf对账结算(Id).Where(x => x.日期 == lastDate); // 最新对账金额

            // 对账后的收发详情
            var listOfFaAfter = listOf出库(Id).Where(x => x.Record.送货日期 > lastDate);
            var listOfShouAfter = listOf入库(Id).Where(x => x.Record.送货日期 > lastDate);
            // 对账后的收款详情
            var listOfShoukuanAfter = listOf收款(Id).Where(x => x.收款日期 > lastDate);
            
            var yingFu = from p in listOfShouAfter
                         let GR = from pp in listOfShouAfter
                                  where pp.Record.单号 == p.Record.单号
                                  select pp
                         select new Models.Model应收应付
                         {
                             应付合计 = GR.Sum(x => x.RecordDetail.金额)
                         };

            return yingFu;
        }
        public static IEnumerable<Models.Model应收应付> 应收应付清单(string Id)
        {
            var finalYingShouYingFu = 应收(Id).Concat(应付(Id));

            return finalYingShouYingFu;

        }

        public static Models.Model应收应付 应收及明细(string Id)
        {
            var last日期 = listOf对账结算(Id).Any() ? listOf对账结算(Id).Max(x => x.日期) : new DateTime(1900,1,1); // 最新对账日期
            var last金额 = listOf对账结算(Id)
                .Where(x => x.日期 == last日期)
                .Select(x => new Models.Model应收应付明细
                {
                    日期 = x.日期,
                    应收 = x.应收,
                    业务描述 = "上期结算",
                    单号 = "Settlement"
                }); // 最新对账金额

            // 对账后的收发详情
            var listOf出库After = listOf出库(Id).Where(x => x.Record.送货日期 > last日期);
            // var listOf入库After = listOf入库(Id).Where(x => x.Record.送货日期 > last日期);
            // 对账后的收款详情
            var listOf收款After = from p in listOf收款(Id)
                                where p.收款日期 > last日期
                                select new Models.Model应收应付明细
                                {
                                    日期 = p.收款日期,
                                    应收 = -p.收款金额,
                                    备注 = p.备注,
                                    业务描述 = "收款",
                                    单号 = "ShouRu"
                                };

            var yingShou = (from p in listOf出库After
                            select new Models.Model应收应付明细
                            {
                                业务类型 = p.Record.业务类型,
                                业务描述 = p.Record.业务描述,
                                日期 = p.Record.送货日期,
                                单号 = p.Record.单号,
                                产品编号 = p.RecordDetail.编号,
                                产品描述 = p.RecordDetail.描述,
                                规格 = p.RecordDetail.规格,
                                片数 = p.RecordDetail.片数,
                                单位数量 = p.RecordDetail.单位数量,
                                计价单位 = p.RecordDetail.计价单位,
                                单价 = p.RecordDetail.单价,
                                应收 = p.RecordDetail.金额,
                                备注 = p.Record.备注
                            })
                           .Union(listOf收款After)
                           .Union(last金额);

            // 应收与明细
            var yingShouWithDetails = new Models.Model应收应付
            {
                单位编号 = Id,
                明细 = yingShou.OrderByDescending(x => x.日期).ThenByDescending(x => x.单号).ToList(),
                应收合计 = yingShou.Sum(x => x.应收)
            };

            return yingShouWithDetails;
        }

        public static Models.Model应收应付 应付及明细(string Id)
        {
            var last日期 = listOf对账结算(Id).Any() ? listOf对账结算(Id).Max(x => x.日期) : new DateTime(1900, 1, 1); // 最新对账日期
            var last金额 = listOf对账结算(Id)
                .Where(x => x.日期 == last日期)
                .Select(x => new Models.Model应收应付明细
                {
                    日期 = x.日期,
                    应付 = x.应付,
                    业务描述 = "上期结算",
                    单号= "Settlement"
                }); // 最新对账金额

            // 对账后的收发详情
            // var listOf出库After = listOf出库(Id).Where(x => x.Record.送货日期 > last日期);
            var listOf入库After = listOf入库(Id).Where(x => x.Record.送货日期 > last日期);
            // 对账后的收款详情
            var listOf付款After = from p in listOf付款(Id)
                                where p.付款日期 > last日期
                                select new Models.Model应收应付明细
                                {
                                    日期 = p.付款日期,
                                    应付 = -p.付款金额,
                                    备注 = p.备注,
                                    业务描述 = "付款",
                                    单号 = "ZhiChu",
                                };

            var yingFu = (from p in listOf入库After
                            select new Models.Model应收应付明细
                            {
                                业务类型 = p.Record.业务类型,
                                业务描述 = p.Record.业务描述,
                                日期 = p.Record.送货日期,
                                单号 = p.Record.单号,
                                产品编号 = p.RecordDetail.编号,
                                产品描述 = p.RecordDetail.描述,
                                规格 = p.RecordDetail.规格,
                                片数 = p.RecordDetail.片数,
                                单位数量 = p.RecordDetail.单位数量,
                                计价单位 = p.RecordDetail.计价单位,
                                单价 = p.RecordDetail.单价,
                                应付 = p.RecordDetail.金额
                            })
                           .Union(listOf付款After)
                           .Union(last金额);

            // 应收与明细
            var yingFuWithDetails = new Models.Model应收应付
            {
                单位编号 = Id,
                明细 = yingFu.OrderByDescending(x => x.日期).ThenByDescending(x => x.单号).ToList(),
                应付合计 = yingFu.Sum(x => x.应付)
            };

            return yingFuWithDetails;
        }

        public static Models.Model应收应付 应收应付及明细(string Id)
        {
            var yingShou = 应收及明细(Id);
            var yingFu = 应付及明细(Id);

            var yingShouYingFu = new Models.Model应收应付
            {
                单位编号 = Id,
                应收合计 = yingShou.应收合计,
                应付合计 = yingFu.应付合计,
                明细 = yingShou.明细.Union(yingFu.明细).OrderByDescending(x => x.日期).ThenByDescending(x => x.单号).ToList()
            };
            
            if (yingShou.明细.FindAll(x => x.业务描述 == "上期结算").Any() && yingFu.明细.FindAll(x => x.业务描述 == "上期结算").Any())
            {
                var row = yingShou.明细.Find(x => x.业务描述 == "上期结算");
                row.应付 = yingFu.明细.Find(x => x.业务描述 == "上期结算").应付;
                yingShouYingFu.明细.RemoveAll(x => x.业务描述 == "上期结算");
                yingShouYingFu.明细.Add(row);
            }
            return yingShouYingFu;
        }
    }
}
