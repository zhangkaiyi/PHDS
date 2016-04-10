using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace PHDS.Models
{
    public class HomeModel
    {
        [Display(Name = "电子邮件")]
        public string Name { get; set; }
    }
}