using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class LevelUserInfoResult
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string PhoneNumber { get; set; }
        public string ReferalCode { get; set; }
        public int TotalUser { get; set; }
    }
}