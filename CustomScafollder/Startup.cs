using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(CustomScafollder.Startup))]
namespace CustomScafollder
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
