using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Rock_Paper_Scissors.Startup))]
namespace Rock_Paper_Scissors
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
