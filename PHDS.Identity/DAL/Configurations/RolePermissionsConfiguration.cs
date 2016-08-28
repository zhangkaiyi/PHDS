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
    public class RolePermissionConfiguration : EntityTypeConfiguration<ApplicationRolePermission>, IEntityMapper
    {
        public RolePermissionConfiguration()
        {
            //配置role与persmission的映射表RolePermission的键
            this.HasKey(r => new { PermissionId = r.PermissionId, RoleId = r.RoleId }).ToTable("RolePermissions");
        }

        void IEntityMapper.RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
