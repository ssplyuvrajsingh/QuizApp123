using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models.Output
{
    public class TransactionModel
    {
        public int TransactionID { get; set; }
        public string UserID { get; set; }
        public System.DateTime? transactionDateTime { get; set; }
        public string UniqueKey { get; set; }
        public string paymentStatus { get; set; }
        public double? amount { get; set; }
        public string comment { get; set; }
        public string username { get; set; }
        public string mobilenumber { get; set; }
        public int? ConvertedPoints { get; set; }
        public string WithdrawType { get; set; }
        public string AccountNumber { get; set; }
        public string NameInAccount { get; set; }
        public string Bank { get; set; }
        public string IFSCCode { get; set; }
        public string PaytmOrderId { get; set; }
        public double? PaytmWithdrawCharges { get; set; }
        public string PaytmResponse { get; set; }

    }
}