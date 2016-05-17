using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TestASPWebServer.Startup))]
namespace TestASPWebServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
