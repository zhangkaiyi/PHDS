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
            [Display(Name = "订单编号")]
            public string OrderId { get; set; }

            [Display(Name = "客户编号")]
            public string CustomerId { get; set; }

            [Display(Name = "客户名称")]
            public string CustomerName { get; set; }
        }
    }
}