using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuizAdmin.Startup))]
namespace QuizAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
