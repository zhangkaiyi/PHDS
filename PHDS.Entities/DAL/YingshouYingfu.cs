using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL
{
    public class YingshouYingfu
    {
        public class Model
        {
            public DateTime? 日期 { get; set; }
            public string 单号 { get; set; }
            public decimal? 应收 { get; set; }
            public decimal? 应付 { get; set; }
        }

        public class ModelShouFa
        {
            public ModelShouFaRecord Record { get; set; }
            public ModelShouFaRecordDetail RecordDetail { get; set; }
        }

        public class ModelShouFaRecord
        {
            public string 单号 { get; set; }
            public string 客户编号 { get; set; }
            public DateTime? 送货日期 { get; set; }
            public string 业务类型 { get; set; }
            public string 业务描述 { get; set; }
        }
        public class ModelShouFaRecordDetail
        {
            public string 编号 { get; set; }
            public string 描述 { get; set; }
            public string 规格 { get; set; }
            public decimal? 片数 { get; set; }
            public string 计价单位 { get; set; }
            public decimal? 单位数量 { get; set; }
            public decimal? 单价 { get; set; }
            public decimal? 金额 { get; set; }
        }

        private static IEnumerable<Edmx.对账结算_主表> detailsOf对账结算(string Id) // 对应Id的对账结算数据
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfDuizhang = from p in pinhua.对账结算_主表
                                        where p.单位编号 == Id
                                        select p;
                return detailsOfDuizhang.ToList();
            }
        }
        private static IEnumerable<Edmx.收款单> detailsOf收款(string Id) // 对应Id的收款单
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfShoukuan = from p in pinhua.收款单
                                        where p.单位编号 == Id
                                        select p;
                return detailsOfShoukuan.ToList();
            }
        }

        private static IEnumerable<ModelShouFa> detailsOf出库(string Id) // 对应Id的发货详情
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfFa = from p1 in pinhua.发货.AsNoTracking()
                                  join p2 in pinhua.发货_DETAIL.AsNoTracking()
                                  on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                  join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                  where p1.客户编号 == Id && p3.MvP == "GI"
                                  select new ModelShouFa
                                  {
                                      Record = new ModelShouFaRecord
                                      {
                                          客户编号 = p1.客户编号,
                                          单号 = p1.送货单号,
                                          送货日期 = p1.送货日期,
                                          业务类型 = p1.业务类型,
                                          业务描述 = p1.业务描述,
                                      },
                                      RecordDetail = new ModelShouFaRecordDetail
                                      {
                                          编号 = p2.编号,
                                          描述 = p2.描述,
                                          规格 = p2.规格,
                                          片数 = p2.PCS,
                                          计价单位 = p2.计价单位,
                                          单位数量 = p2.单位数量,
                                          单价 = p2.单价,
                                          金额 = p2.金额
                                      }
                                  };
                return detailsOfFa.ToList();
            }
        }

        private static IEnumerable<ModelShouFa> detailsOf入库(string Id) // 对应Id的收货详情
        {
            using (var pinhua = new Edmx.PinhuaEntities())
            {
                var detailsOfShou1 = from p1 in pinhua.发货.AsNoTracking()
                                     join p2 in pinhua.发货_DETAIL.AsNoTracking()
                                     on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                     join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                     where p1.客户编号 == Id && p3.MvP == "GR"
                                     select new ModelShouFa
                                     {
                                         Record = new ModelShouFaRecord
                                         {
                                             客户编号 = p1.客户编号,
                                             单号 = p1.送货单号,
                                             送货日期 = p1.送货日期,
                                             业务类型 = p1.业务类型,
                                             业务描述 = p1.业务描述,
                                         },
                                         RecordDetail = new ModelShouFaRecordDetail
                                         {
                                             编号 = p2.编号,
                                             描述 = p2.描述,
                                             规格 = p2.规格,
                                             片数 = p2.PCS,
                                             计价单位 = p2.计价单位,
                                             单位数量 = p2.单位数量,
                                             单价 = p2.单价,
                                             金额 = p2.金额
                                         }
                                     };
                var detailsOfShou2 = from p1 in pinhua.收货.AsNoTracking()
                                     join p2 in pinhua.收货_D.AsNoTracking()
                                     on p1.ExcelServerRCID equals p2.ExcelServerRCID
                                     join p3 in pinhua.业务类型.AsNoTracking() on p1.业务类型 equals p3.业务类型1
                                     where p1.供应商编号 == Id && p3.MvP == "GR"
                                     select new ModelShouFa
                                     {
                                         Record = new ModelShouFaRecord
                                         {
                                             客户编号 = p1.供应商编号,
                                             单号 = p1.单号,
                                             送货日期 = p1.日期,
                                             业务类型 = p1.业务类型,
                                             业务描述 = p1.业务描述,
                                         },
                                         RecordDetail = new ModelShouFaRecordDetail
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

        public static IEnumerable<Model> YingShou(string Id)
        {
            var lastDate = detailsOf对账结算(Id).Max(x => x.日期); // 最新对账日期
            var lastAmount = detailsOf对账结算(Id).Where(x => x.日期 == lastDate); // 最新对账金额

            // 对账后的收发详情
            var detailsOfFaAfter = detailsOf出库(Id).Where(x => x.Record.送货日期 > lastDate);
            var detailsOfShouAfter = detailsOf入库(Id).Where(x => x.Record.送货日期 > lastDate);
            // 对账后的收款详情
            var detailsOfShoukuanAfterLastest = detailsOf收款(Id).Where(x => x.收款日期 > lastDate);

            var yingShou = from p in detailsOfFaAfter
                           let GI = from pp in detailsOfFaAfter
                                    where pp.Record.单号 == p.Record.单号
                                    select pp
                           select new Model
                           {
                               日期 = p.Record.送货日期,
                               单号 = p.Record.单号,
                               应收 = GI.Sum(x => x.RecordDetail.金额),
                           };

            return yingShou;
        }
        public static IEnumerable<Model> YingFu(string Id)
        {
            var lastDate = detailsOf对账结算(Id).Max(x => x.日期); // 最新对账日期
            var lastAmount = detailsOf对账结算(Id).Where(x => x.日期 == lastDate); // 最新对账金额

            // 对账后的收发详情
            var detailsOfFaAfter = detailsOf出库(Id).Where(x => x.Record.送货日期 > lastDate);
            var detailsOfShouAfter = detailsOf入库(Id).Where(x => x.Record.送货日期 > lastDate);
            // 对账后的收款详情
            var detailsOfShoukuanAfterLastest = detailsOf收款(Id).Where(x => x.收款日期 > lastDate);
            
            var yingFu = from p in detailsOfShouAfter
                         let GR = from pp in detailsOfShouAfter
                                  where pp.Record.单号 == p.Record.单号
                                  select pp
                         select new Model
                         {
                             日期 = p.Record.送货日期,
                             单号 = p.Record.单号,
                             应付 = GR.Sum(x => x.RecordDetail.金额)
                         };

            return yingFu;
        }
        public static IEnumerable<Model> YingShouYingFu(string Id)
        {
            var finalYingShouYingFu = YingShou(Id).Concat(YingFu(Id));

            return finalYingShouYingFu;

        }
    }
}
