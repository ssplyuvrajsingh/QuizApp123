using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class WithdrawalAmountModel
    {
        public string UserId { get; set; }
        public string AccountNumber { get; set; }
        public string NameInAccount { get; set; }
        public string Bank { get; set; }
        public string IFSCCode { get; set; }
        public double amount { get; set; }
        public string WithdrawType { get; set; }
        public string Passcode { get; set; }
    }
    public class WithdrawalAmountBalance
    {
        public double Balance { get; set; }
        public string State { get; set; }
        public string status { get; set; }
    }
}