using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TeamWork.Startup))]
namespace TeamWork
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
