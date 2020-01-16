using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class PointsRedeemModel
    {
        public string UserID { get; set; }
        public int PointsWithdraw { get; set; }
        public string Passcode { get; set; }
    }
    public class RedeemBalanceModel
    {
        public double RedeemBalance { get; set; }
        public string State { get; set; }
    }
}