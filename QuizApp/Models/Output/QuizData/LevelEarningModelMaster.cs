using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class LevelEarningModelMaster
    {
        public List<LevelEarningModel> levelEarnings { get; set; }
    }

    public class LevelEarningModel
    {
        public string Title { get; set; }
        public double Amount { get; set; }
        public int Activeuser { get; set; }
    }
    public class LevelUsers
    {
        public string UserID { get; set; }
    }
}