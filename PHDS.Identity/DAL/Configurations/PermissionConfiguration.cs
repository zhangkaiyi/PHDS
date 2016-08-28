using PHDS.Identity.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.ModelConfiguration.Configuration;

namespace PHDS.Identity.DAL.Configurations
{
    public class PermissionConfiguration : EntityTypeConfiguration<ApplicationPermission>, IEntityMapper
    {
        public PermissionConfiguration()
        {
            //配置permission与rolePermission的1对多关系
            this.ToTable("Permissions");
            this.HasMany<ApplicationRolePermission>(u => u.Roles).WithRequired().HasForeignKey(k => k.PermissionId);
        }

        void IEntityMapper.RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
