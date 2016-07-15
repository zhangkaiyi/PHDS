using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL.应收应付
{
    public class Models
    {
        public class Model应收应付
        {
            public string 单位编号 { get; set; }
            public decimal? 应收合计 { get; set; }
            public decimal? 应付合计 { get; set; }
            public List<Model应收应付明细> 明细 { get; set; }
        }

        public class Model应收应付明细
        {
            public string 业务类型 { get; set; }
            public string 业务描述 { get; set; }
            public DateTime? 日期 { get; set; }
            public string 单号 { get; set; }
            public string 产品编号 { get; set; }
            public string 产品描述 { get; set; }
            public string 规格 { get; set; }
            public decimal? 片数 { get; set; }
            public decimal? 单位数量 { get; set; }
            public string 计价单位 { get; set; }
            public decimal? 单价 { get; set; }
            public decimal? 应收 { get; set; }
            public decimal? 应付 { get; set; }
            public string 备注 { get; set; }
            public decimal? 单据应收合计 { get; set; }
            public decimal? 单据应付合计 { get; set; }
        }

        public class Model出库入库
        {
            public Model出库入库条目 Record { get; set; }
            public Model出库入库明细 RecordDetail { get; set; }
        }
        public class Model出库入库条目
        {
            public string 单号 { get; set; }
            public string 客户编号 { get; set; }
            public DateTime? 送货日期 { get; set; }
            public string 业务类型 { get; set; }
            public string 业务描述 { get; set; }
            public string 备注 { get; set; }
        }
        public class Model出库入库明细
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
    }
}
