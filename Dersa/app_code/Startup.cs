using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DersaApplication.Startup))]
namespace DersaApplication
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
