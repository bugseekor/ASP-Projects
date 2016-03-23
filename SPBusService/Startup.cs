using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SPBusService.Startup))]
namespace SPBusService
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
