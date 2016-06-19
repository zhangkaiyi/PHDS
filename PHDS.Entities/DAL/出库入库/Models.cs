using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Entities.DAL.出库入库
{
    public class Models
    {
        public class 在途物料
        {
            public string 单位编号 { get; set; }
            public string 单位名称 { get; set; }
            public string 产品编号 { get; set; }
            public string 产品描述 { get; set; }
            public string 规格 { get; set; }
            public decimal 数量 { get; set; }
        }
    }
}
