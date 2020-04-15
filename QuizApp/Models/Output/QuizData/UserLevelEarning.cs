using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
   
    public class UserLevelEarning
    {
        public int ID { get; set; }
        public string UserID { get; set; }
        public string ParentIDs { get; set; }
        public string Name { get; set; }
        public Nullable<System.DateTime> LastActiveDate { get; set; }
        public string ReferalCode { get; set; }
        public string UsedReferalCode { get; set; }
        public Nullable<bool> isActive { get; set; }
    }

    public class UserLevelList
    {
        public List<UserLevelEarning> userLevelEarnings { get; set; }
    }

    public class SetLevelForParentUser
    {
        public string UserId { get; set; }
        public int Level { get; set; }
        public int Count { get; set; }
        public DateTime LastUpdate {get;set;}
    }

    public class EaningHeadModel
    {
        public int RegistrationIncome { get; set; }
        public double DirectIncome { get; set; }
        public double Level1Income { get; set; }
        public double Level2Income { get; set; }
        public double Level3Income { get; set; }
        public double Level4Income { get; set; }
        public double Level5Income { get; set; }
        public double Level6Income { get; set; }
        public double Level7Income { get; set; }
        public double Level8Income { get; set; }
        public double Level9Income { get; set; }
        public double Level10Income { get; set; }
        public int PaytmMinimumWithdrawlLimit { get; set; }
        public int BankMinimumWithdrawlLimit { get; set; }
        public int MinimumCharges { get; set; }
        public int MaximumWithdrawLimit { get; set; }
        public int ActiveHourLimit { get; set; }
        public double PointAmount { get; set; }
        public int WithdrawCharges { get; set; }
        public int MinimumQuiz { get; set; }
        public int MaxQuiz { get; set; }
        public int Rewards { get; set; }  
    }

    public class LevelWithUser
    {
        public int Level { get; set; }
        public List<ChildUser> ChildUsers { get; set; }
    }
    public class ChildUser
    {
        public string UserId { get; set; }
    }

    public class LevelUsersModel
    {
        public Nullable<int> Level1Users { get; set; }
        public Nullable<int> Level2Users { get; set; }
        public Nullable<int> Level3Users { get; set; }
        public Nullable<int> Level4Users { get; set; }
        public Nullable<int> Level5Users { get; set; }
        public Nullable<int> Level6Users { get; set; }
        public Nullable<int> Level7Users { get; set; }
        public Nullable<int> Level8Users { get; set; }
        public Nullable<int> Level9Users { get; set; }
        public Nullable<int> Level10Users { get; set; }
        public Nullable<double> Level1 { get; set; }
        public Nullable<double> Level2 { get; set; }
        public Nullable<double> Level3 { get; set; }
        public Nullable<double> Level4 { get; set; }
        public Nullable<double> Level5 { get; set; }
        public Nullable<double> Level6 { get; set; }
        public Nullable<double> Level7 { get; set; }
        public Nullable<double> Level8 { get; set; }
        public Nullable<double> Level9 { get; set; }
        public Nullable<double> Level10 { get; set; }
        public Nullable<System.DateTime> LastUpdate { get; set; }
    }
}