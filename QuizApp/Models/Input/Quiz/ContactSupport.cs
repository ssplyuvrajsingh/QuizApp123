using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input.Quiz
{
    public class ContactSupportModel
    {
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public string UserMessage { get; set; }
        public string ciphertoken { get; set; }
    }
}