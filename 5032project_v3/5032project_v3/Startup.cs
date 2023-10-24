using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(_5032project_v3.Startup))]
namespace _5032project_v3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
