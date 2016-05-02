using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PHDS.Web.Models
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
            public DateTime? SalesDate { get; set; }

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

            [Display(Name = "金额")]
            public decimal SalesAmount { get; set; }
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
            public string Description { get; set; }

            [Required]
            [Display(Name = "规格")]
            public string 规格 { get; set; }

            [Required]
            [Display(Name = "片数")]
            [RegularExpression(@"^(([0-9]*)|([0-9]*\.[0-9]*))$")]
            public string PCS { get; set; }

            [Required]
            [Display(Name = "单位数量")]
            [RegularExpression(@"^(([0-9]*)|([0-9]*\.[0-9]*))$")]
            public string UnitQuantity { get; set; }

            [Required]
            [Display(Name = "计价单位")]
            public string ChargeUnit { get; set; }
            
            [Display(Name = "单价")]
            [RegularExpression(@"^(([0-9]*)|([0-9]*\.[0-9]*))$")]
            public string Price { get; set; }

            [Display(Name = "金额")]
            [RegularExpression(@"^(([0-9]*)|([0-9]*\.[0-9]*))$")]
            public string Amount { get; set; }

            [Display(Name = "木种")]
            public string WoodSpecies { get; set; }

            [Display(Name = "工艺")]
            public string Processing { get; set; }

            public string RCID { get; set; }
        }
    }
}