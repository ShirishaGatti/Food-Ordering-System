using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ItemListTask1.Startup))]
namespace ItemListTask1
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
