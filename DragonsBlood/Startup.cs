using System.Configuration;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DragonsBlood.Startup))]
namespace DragonsBlood
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            GlobalHost.DependencyResolver.UseSqlServer(
                ConfigurationManager.ConnectionStrings[ConfigurationManager.AppSettings["ConnectionPropertyName"]].ToString());
            app.MapSignalR(new HubConfiguration() {EnableDetailedErrors = true});
        }
    }
}
