using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class FillReferalModel
    {
        public string ReferalCode { get; set; }
        public string IpAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceModel { get; set; }
        public bool IsUsed { get; set; }
    }
}