using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using PHDS.Identity.BLL;
using PHDS.Identity.Interface;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PHDS.Identity.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        static ApplicationDbContext()
        {
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());
        }

        private static ICollection<IEntityMapper> GetAllEntityMapper()
        {
            ICollection<Assembly> EntityMapperAssemblies = EntityMapperAssemblies = new[]
                        {
                            Assembly.LoadFrom(Path.Combine(AppDomain.CurrentDomain.RelativeSearchPath, "PHDS.Identity.dll"))
                            //Assembly.Load(Assembly.GetExecutingAssembly().FullName)
                        };
            if (EntityMapperAssemblies.Count == 0)
            {
                throw new InvalidOperationException("上下文“{0}”初始化失败，请添加实体映射程序集");
            }
            Type baseType = typeof(IEntityMapper);
            Type[] mapperTypes = EntityMapperAssemblies.SelectMany(assembly => assembly.GetTypes())
                .Where(type => baseType.IsAssignableFrom(type) && type != baseType && !type.IsAbstract).ToArray();
            ICollection<IEntityMapper> result = mapperTypes.Select(type => Activator.CreateInstance(type) as IEntityMapper).ToList();
            return result;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            base.OnModelCreating(modelBuilder);

            ////配置permission与rolePermission的1对多关系
            //EntityTypeConfiguration<ApplicationPermission> configuration = modelBuilder.Entity<ApplicationPermission>().ToTable("ApplicationPermissions");
            //configuration.HasMany<ApplicationRolePermission>(u => u.Roles).WithRequired().HasForeignKey(k => k.PermissionId);
            ////配置role与persmission的映射表RolePermission的键
            //modelBuilder.Entity<ApplicationRolePermission>().HasKey(r => new { PermissionId = r.PermissionId, RoleId = r.RoleId }).ToTable("RolePermissions");
            ////配置role与RolePermission的1对多关系
            //EntityTypeConfiguration<ApplicationRole> configuration2 = modelBuilder.Entity<ApplicationRole>();
            //configuration2.HasMany<ApplicationRolePermission>(r => r.Permissions).WithRequired().HasForeignKey(k => k.RoleId);

            //modelBuilder.Configurations.Add(new Configurations.PermissionConfiguration());
            //modelBuilder.Configurations.Add(new Configurations.RolePermissionConfiguration());
            //modelBuilder.Configurations.Add(new Configurations.RoleConfiguration());

            IEnumerable<IEntityMapper> EntityMappers = GetAllEntityMapper();
            if (EntityMappers == null)
            {
                return;
            }
            foreach (var mapper in EntityMappers)
            {
                mapper.RegistTo(modelBuilder.Configurations);
            }
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public new IDbSet<ApplicationRole> Roles { get; set; }

        public IDbSet<ApplicationPermission> Permissions { get; set; }

    }
    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }

        //Create User=Admin@Admin.com with password=Admin@123456 in the Admin role        
        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>();
            var roleManager = HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>();
            var users = new List<InitModel>
            {
                new InitModel { Name="张凯译",Password="benny0922",Email="116307766@qq.com"},
                new InitModel { Name="高兴",Password="Haoxuan0209",Email="274158227@qq.com"},
                new InitModel { Name="沈嘉欢",Password="sjh690221SJH",Email="294128225@qq.com"},
                new InitModel { Name="唐丽娜",Password="woaitln88A",Email="9433985@qq.com"},
                new InitModel { Name="李",Password="890989l",Email="libo880630@126.com"},
            };
            const string roleName1 = "管理员";
            const string roleName2 = "对账员";

            //Create Role Admin if it does not exist
            var role1 = roleManager.FindByName(roleName1);
            if (role1 == null)
            {
                role1 = new ApplicationRole(roleName1);
                var roleresult = roleManager.Create(role1);
            }
            var role2 = roleManager.FindByName(roleName2);
            if (role2 == null)
            {
                role2 = new ApplicationRole(roleName2);
                var roleresult = roleManager.Create(role2);
            }
            foreach(var model in users)
            {
                var user = userManager.FindByName(model.Name);
                if (user == null)
                {
                    user = new ApplicationUser { UserName = model.Name, Email = model.Email, Password = model.Password };
                    var result = userManager.Create(user, model.Password);
                    result = userManager.SetLockoutEnabled(user.Id, false);
                }

                // Add user admin to Role Admin if not already added
                var rolesForUser = userManager.GetRoles(user.Id);
                if (user.UserName == "张凯译")
                {
                    if (!rolesForUser.Contains(role1.Name))
                    {
                        var result = userManager.AddToRole(user.Id, role1.Name);
                    }
                }
            }
        }
    }

    public class InitModel
    {
        public string Name { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }
}
