using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using PHDS.Identity.DAL;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PHDS.Identity.BLL
{
    public class ApplicationRoleManager : RoleManager<ApplicationRole>
    {
        public ApplicationRoleManager(IRoleStore<ApplicationRole, string> roleStore)
            : base(roleStore)
        {
        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            return new ApplicationRoleManager(new RoleStore<ApplicationRole>(context.Get<ApplicationDbContext>()));
        }

        /// <summary>
        /// 获取角色的权限列表
        /// </summary>
        /// <param name="roleId">角色ID</param>
        /// <returns>权限ID列表的IQueryable</returns>
        public IEnumerable<ApplicationPermission> GetRolePermissions(string roleId)
        {
            //取数据上下文
            var context = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            //取角色
            var role = context.Roles.Include(r => r.Permissions).FirstOrDefault(t => t.Id == roleId);
            //取权限ID列表        
            var rolePermissionIds = role.Permissions.Select(t => t.PermissionId);
            //取权限列表
            var permissions = context.Permissions.Where(p => rolePermissionIds.Contains(p.Id)).ToList();
            return permissions;
        }

        /// <summary>
        /// 取用户的权限列表
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public IEnumerable<ApplicationPermission> GetUserPermissions(string username)
        {
            //用户权限集合
            var userPermissions = new List<ApplicationPermission>();
            //取数据上下文
            var context = HttpContext.Current.GetOwinContext().Get<ApplicationDbContext>();
            //取用户
            var user = context.Users.Include(u => u.Roles)
                              .FirstOrDefault(t => t.UserName.ToUpper() == username.ToUpper());
            //取用户所属角色的所有权限
            foreach (var item in user.Roles)
            {
                //取角色权限
                var rolePermissions = GetRolePermissions(item.RoleId);
                //插入用户权限
                userPermissions.InsertRange(userPermissions.Count, rolePermissions);
            }
            return userPermissions;
        }

    }
}
