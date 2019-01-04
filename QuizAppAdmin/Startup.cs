using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuizAppAdmin.Startup))]
namespace QuizAppAdmin
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
