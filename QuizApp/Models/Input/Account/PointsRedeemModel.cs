using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class PointsRedeemModel
    {
        public string UserID { get; set; }
        public DateTime TransactionDate { get; set; }
        public int PointsEarned { get; set; }
        public int PointsWithdraw { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}