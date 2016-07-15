using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Identity.DAL
{
    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string name) : base(name) { }

        /// <summary>
        /// 角色描述
        /// </summary>
        [DisplayName("角色描述")]
        public string Description { get; set; }
    }
}
