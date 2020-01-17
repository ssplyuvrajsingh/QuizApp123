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
            var jsonfilepath = HttpContext.Current.Server.MapPath("~/models/jsonfile/levelearningmasteruser.json");
            if (!string.IsNullOrEmpty(jsonfilepath))
            {
                //    //GetHangfireServers(app, jsonFilePath);
            }
        }
        /// <summary>
        /// This function will be callled every day at 2.00 am
        /// </summary>
        /// <param name="app"></param> 
        /// <param name="jsonFilePath">Read json file</param>
        private void GetHangfireServers(IAppBuilder app, string jsonfilepath)
        {
            //GlobalConfiguration.Configuration.UseSqlServerStorage("defaultconnection");
            //QuizBinding quizbinding = new QuizBinding();
            ////RecurringJob.AddOrUpdate(() => quizbinding.AddLevelBaseEarningAmount(jsonfilepath), Cron.Daily(20, 30));

            //app.UseHangfireServer();
            //app.UseHangfireDashboard();
        }


    }
}
