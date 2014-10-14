using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MVC_DATABASE.Startup))]
namespace MVC_DATABASE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
