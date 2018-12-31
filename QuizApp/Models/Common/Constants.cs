using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class Constants
    {
        public static string SiteUrl = ConfigurationManager.AppSettings["SiteUrl"];

        public static int TimeOfExpireRefreshTokenHours = 1;

        public static string Token = SiteUrl + "/token";
    }
}