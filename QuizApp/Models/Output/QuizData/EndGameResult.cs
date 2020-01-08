using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Output.QuizData
{
    public class EndGameResult
    {
        public int PointsEarned { get; set; }
        public int PercentageEarn { get; set; }
        public string TimeTakeninSeconds { get; set; }
        public bool IsWon { get; set; }
    }
}