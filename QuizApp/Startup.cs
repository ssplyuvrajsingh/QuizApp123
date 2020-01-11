using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.Owin;
using Owin;
using QuizApp.Models;

[assembly: OwinStartup(typeof(QuizApp.Startup))]

namespace QuizApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
            if (!string.IsNullOrEmpty(jsonFilePath))
            {
                GetHangfireServers(app, jsonFilePath);
            }
        }
        /// <summary>
        /// This function will be callled every day at 2.00 am
        /// </summary>
        /// <param name="app"></param>
        /// <param name="jsonFilePath">Read json file</param>
        private void GetHangfireServers(IAppBuilder app, string jsonFilePath)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("DefaultConnection");
            QuizBinding quizBinding = new QuizBinding();
            RecurringJob.AddOrUpdate(() => quizBinding.AddLevelBaseEarningAmount(jsonFilePath), Cron.Daily(20, 30));

            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }


    }
}
