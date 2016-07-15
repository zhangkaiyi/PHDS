using Owin;
using Microsoft.Owin;
using PHDS.Identity.DAL;
using System.Data.Entity;

[assembly: OwinStartup(typeof(PHDS.Web.Startup))]
namespace PHDS.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}