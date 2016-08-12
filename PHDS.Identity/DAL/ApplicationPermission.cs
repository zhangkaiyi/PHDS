using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PHDS.Identity.DAL
{
    public class ApplicationPermission
    {
        public ApplicationPermission()
        {
            Id = Guid.NewGuid().ToString();
            Roles = new List<ApplicationRolePermission>();
        }
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 控制器名
        /// </summary>
        public string Controller { get; set; }
        /// <summary>
        /// 方法名
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 参数字符串
        /// </summary>
        public string Params { get; set; }
        /// <summary>
        /// 功能描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 角色列表
        /// </summary>
        public ICollection<ApplicationRolePermission> Roles { get; set; }


    }

    public class ApplicationPermissionEqualityComparer : IEqualityComparer<ApplicationPermission>
    {
        public bool Equals(ApplicationPermission x, ApplicationPermission y)
        {
            //先比较ID
            if (string.Compare(x.Id, y.Id, true) == 0)
            {
                return true;
            }
            //而后比较Controller,Action,Description和Params
            if (x.Controller == y.Controller || x.Action == y.Action || x.Description == y.Description)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(ApplicationPermission obj)
        {
            var str = string.Format("{0}-{1}-{2}", obj.Controller, obj.Action, obj.Description);
            return str.GetHashCode();
        }
    }

    public class ApplicationRolePermission
    {
        public virtual string RoleId { get; set; }
        public virtual string PermissionId { get; set; }
    }

}
