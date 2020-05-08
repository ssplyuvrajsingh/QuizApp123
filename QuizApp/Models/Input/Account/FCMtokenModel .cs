using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class FCMtokenModel
    {
        public string UserID { get; set; }
        public string FCMToken { get; set; }
        public string ciphertoken { get; set; }
    }
}