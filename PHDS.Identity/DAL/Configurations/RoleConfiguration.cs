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
    public class RoleConfiguration : EntityTypeConfiguration<ApplicationRole>, IEntityMapper
    {
        public RoleConfiguration()
        {
            this.HasMany<ApplicationRolePermission>(r => r.Permissions).WithRequired().HasForeignKey(k => k.RoleId);
        }

        void IEntityMapper.RegistTo(ConfigurationRegistrar configurations)
        {
            configurations.Add(this);
        }
    }
}
