using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PHDS.Models
{
    public class SalesModels
    {
        public class OrdersModel
        {
            [Required]
            [Display(Name = "RCID")]
            public string RCID { get; set; }

            [Display(Name = "订单编号")]
            public string OrderId { get; set; }

            [Display(Name = "销售日期")]
            public DateTime SalesDate { get; set; }

            [Display(Name = "客户编号")]
            public string CustomerId { get; set; }

            [Display(Name = "客户名称")]
            public string CustomerName { get; set; }

            [Display(Name = "客户地址")]
            public string CustomerAddress { get; set; }

            [Display(Name = "销售类型")]
            public string SalesTypeId { get; set; }

            [Display(Name = "类型描述")]
            public string SalesTypeDescription { get; set; }

            [Display(Name = "备注")]
            public string SalesComment { get; set; }
            public List<OrderDetialModel> Details { get; set; }
        }

        public class OrderDetialModel
        {
            [Required]
            [Display(Name = "行号")]
            public int RN { get; set; }

            [Required]
            [Display(Name = "编号")]
            public string Id { get; set; }

            [Required]
            [Display(Name = "名称")]
            public string 描述 { get; set; }

            [Required]
            [Display(Name = "规格")]
            public string 规格 { get; set; }

            [Required]
            [Display(Name = "片数")]
            public int PCS { get; set; }

            [Required]
            public decimal 单位数量 { get; set; }

            [Required]
            public string 计价单位 { get; set; }

            public decimal 单价 { get; set; }

            public decimal 金额 { get; set; }

            public string 木种 { get; set; }

            public string 工艺 { get; set; }

            public string RCID { get; set; }
        }
    }
}