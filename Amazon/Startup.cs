using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Amazon.Startup))]
namespace Amazon
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
