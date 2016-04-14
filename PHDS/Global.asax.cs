using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace PHDS
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Entity Framework 预热
            using (var dbcontext = new PHDS.EF.PinhuaEntities())
            {
                var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbcontext).ObjectContext;
                var mappingCollection = (System.Data.Entity.Core.Mapping.StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<System.Data.Entity.Core.Metadata.Edm.EdmSchemaError>());
            }

            using (var dbcontext = new PHDS.EF.EastRiverEntities())
            {
                var objectContext = ((System.Data.Entity.Infrastructure.IObjectContextAdapter)dbcontext).ObjectContext;
                var mappingCollection = (System.Data.Entity.Core.Mapping.StorageMappingItemCollection)objectContext.MetadataWorkspace.GetItemCollection(System.Data.Entity.Core.Metadata.Edm.DataSpace.CSSpace);
                mappingCollection.GenerateViews(new List<System.Data.Entity.Core.Metadata.Edm.EdmSchemaError>());
            }

        }
    }
}