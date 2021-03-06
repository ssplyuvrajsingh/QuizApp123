﻿using System;
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
           GetHangfireServers(app);
           GetHangfireServersLevel(app);
           //var jsonfilepath = HttpContext.Current.Server.MapPath("~/models/jsonfile/levelearningmasteruser.json");
        }
        /// <summary>
        /// This function will be callled every day at 2.00 am
        /// </summary>
        /// <param name="app"></param> 
        /// <param name="jsonFilePath">Read json file</param>
        private void GetHangfireServersLevel(IAppBuilder app)
        {
        GlobalConfiguration.Configuration.UseSqlServerStorage("defaultconnection");
        QuizBinding quizbinding = new QuizBinding();
        RecurringJob.AddOrUpdate(() => quizbinding.CallLevelBaseEarningAmount(), Cron.Daily(20,30));

        app.UseHangfireServer();
        app.UseHangfireDashboard();
        }
        private void GetHangfireServers(IAppBuilder app)
        {
            GlobalConfiguration.Configuration.UseSqlServerStorage("defaultconnection");
            PaytmBinding paytmBinding = new PaytmBinding();
            RecurringJob.AddOrUpdate(() => paytmBinding.paytmJob(), Cron.Hourly());

            app.UseHangfireServer();
            app.UseHangfireDashboard();
        }

    }
}
