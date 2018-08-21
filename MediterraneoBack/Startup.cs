using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MediterraneoBack.Startup))]
namespace MediterraneoBack
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
