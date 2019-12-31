using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models.Output
{
    public class AllUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Platform { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string ReferalCode { get; set; }
        public string UsedReferalCode { get; set; }
        public string ParentIDs { get; set; }
    }
}