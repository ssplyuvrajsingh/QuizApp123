using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class AddBankAccountDetailsModel
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string NameInAccount { get; set; }
        public string Bank { get; set; }
        public string IFSCCode { get; set; }
        public double amount { get; set; }
        public string WithdrawType { get; set; }
    }
}