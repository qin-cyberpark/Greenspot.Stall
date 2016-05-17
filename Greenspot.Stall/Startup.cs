using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Greenspot.Stall.Startup))]

namespace Greenspot.Stall
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
