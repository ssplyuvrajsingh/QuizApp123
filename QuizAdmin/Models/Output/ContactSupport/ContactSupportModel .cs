using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class ContactSupportModel
    {
        public int ContactSupportId { get; set; }
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public string UserMessage { get; set; }
    }
}