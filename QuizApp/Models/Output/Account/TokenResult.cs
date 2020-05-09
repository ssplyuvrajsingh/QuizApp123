using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class TokenResult
    {
        public bool result { get; set; }
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string error_message { get; set; }
        public string id { get; set; }
        public string RefferalCode { get; set; }
        public string MobileNumber { get; set; }
        public string UserName { get; set; }
        public string FCMToken { get; set; }
    }
}