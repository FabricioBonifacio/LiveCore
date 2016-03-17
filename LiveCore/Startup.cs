using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LiveCore.Startup))]
namespace LiveCore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
