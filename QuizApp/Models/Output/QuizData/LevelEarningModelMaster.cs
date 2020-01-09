using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class LevelEarningModelMaster
    {
        public Nullable<double> Level1Amount { get; set; }
        public Nullable<double> Level2Amount { get; set; }
        public Nullable<double> Level3Amount { get; set; }
        public Nullable<double> Level4Amount { get; set; }
        public Nullable<double> Level5Amount { get; set; }
        public Nullable<double> Level6Amount { get; set; }
        public Nullable<double> Level7Amount { get; set; }
        public Nullable<double> Level8Amount { get; set; }
        public Nullable<double> Level9Amount { get; set; }
        public Nullable<double> Level10Amount { get; set; }
        public Nullable<int> Level1ActiveUsers { get; set; }
        public Nullable<int> Level2ActiveUsers { get; set; }
        public Nullable<int> Level3ActiveUsers { get; set; }
        public Nullable<int> Level4ActiveUsers { get; set; }
        public Nullable<int> Level5ActiveUsers { get; set; }
        public Nullable<int> Level6ActiveUsers { get; set; }
        public Nullable<int> Level7ActiveUsers { get; set; }
        public Nullable<int> Level8ActiveUsers { get; set; }
        public Nullable<int> Level9ActiveUsers { get; set; }
        public Nullable<int> Level10ActiveUsers { get; set; }
    }
    public class LevelUsers
    {
        public string UserID { get; set; }
    }
}