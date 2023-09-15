using Microsoft.Owin;
using Owin;
using project_v2.Models;
using System.Data.Entity;

[assembly: OwinStartupAttribute(typeof(project_v2.Startup))]
namespace project_v2
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {

            Database.SetInitializer<ApplicationDbContext>(null);

            ConfigureAuth(app);
        }
    }
}
