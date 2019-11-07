using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Output.QuizData
{
    public class GetScoreResult
    {
        public int YourBestScore { get; set; }
        public int OverallScore { get; set; }
    }
}