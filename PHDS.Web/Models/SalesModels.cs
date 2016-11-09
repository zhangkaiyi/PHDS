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
            [Display(Name = "rcId")]
            public string rcId { get; set; }

            [Display(Name = "单据编号")]
            public string orderId { get; set; }

            [Display(Name = "出库日期")]
            public DateTime? stockoutDate { get; set; }

            [Display(Name = "客户编号")]
            public string customerId { get; set; }

            [Display(Name = "客户名称")]
            public string customerName { get; set; }

            [Display(Name = "客户地址")]
            public string customerAddress { get; set; }

            [Display(Name = "业务类型")]
            public string stockoutType { get; set; }

            [Display(Name = "类型描述")]
            public string stockoutTypeDescription { get; set; }

            [Display(Name = "备注")]
            public string stockoutComment { get; set; }

            [Display(Name = "金额")]
            public decimal stockoutAmount { get; set; }

            [Display(Name = "面积")]
            public decimal stockoutSquare { get; set; }

            [Display(Name = "项数")]
            public int itemsCount { get; set; }
        }

        public class OrderDetailModel
        {
            [Required]
            [Display(Name = "行号")]
            public int? RN { get; set; }

            [Required]
            [Display(Name = "编号")]
            public string Id { get; set; }

            [Required]
            [Display(Name = "名称")]
            public string Description { get; set; }

            [Required]
            [Display(Name = "规格")]
            public string Size { get; set; }

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
        
        public class Customer
        {
            public int Rank { get; set; }
            public string Id { get; set; }
            public string Name { get; set; }
        }
    }
}