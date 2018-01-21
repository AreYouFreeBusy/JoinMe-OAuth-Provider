using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(JoinMe_OAuth_Demo.Startup))]
namespace JoinMe_OAuth_Demo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
