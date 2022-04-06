using System.Data.Entity;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using DragonsBlood.AppMigrations;
using DragonsBlood.Data;

namespace DragonsBlood
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            if(!HttpContext.Current.IsDebuggingEnabled)
                BundleTable.EnableOptimizations = true;

            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ResourcesDbContext, Data.ResourceMigrations.Configuration>());
        }
    }
}
