using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class UserProfileModel
    {
        public string ReferalCode { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string UsedReferalCode { get; set; }
        public string ParentName { get; set; }
        public string ParentPhoneNumber { get; set; }
    }
}