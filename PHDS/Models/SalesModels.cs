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
            [Display(Name = "唯一标识")]
            public string RCID { get; set; }

            [Display(Name = "订单编号")]
            public string OrderId { get; set; }

            [Display(Name = "销售日期")]
            public DateTime SaleDate { get; set; }

            [Display(Name = "客户编号")]
            public string CustomerId { get; set; }

            [Display(Name = "客户名称")]
            public string CustomerName { get; set; }

            [Display(Name = "客户地址")]
            public string CustomerAddress { get; set; }

            [Display(Name = "销售类型")]
            public string SalesTypeId { get; set; }

            [Display(Name = "销售类型描述")]
            public string SalesTypeDescription { get; set; }

        }
    }
}