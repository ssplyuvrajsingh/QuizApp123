using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class UserWalletModel
    {
        public double CurrentBalance { get; set; }

        public double MothlyIncome { get; set; }
        public double TotalWithdraw { get; set; }
        public int TotalPoins { get; set; }
        public List<TransactionModel> TransactionModels { get; set; }
    }
    public class TransactionModel
    {
        public string transactionDateTime { get; set; }
        public string paymentStatus { get; set; }
        public double amount { get; set; }
    }
    public class UserWalletInput
    {
        public string UserId { get; set; }
    }
}