using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Output.QuizData
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
        public int MinimumWithdrawlLimit { get; set; }
        public int MinimumCharges { get; set; }
        public int MaximumWithdrawLimit { get; set; }
        public int ActiveHourLimit { get; set; }
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
}