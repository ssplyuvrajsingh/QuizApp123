using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Output.QuizData
{
    public class QuizResult
    {
        public Guid QuizID { get; set; }
        public string QuizTitle { get; set; }
        public string PlayingDescriptionImg { get; set; }
        public DateTime QuizDate { get; set; }
        public string QuizBannerImage { get; set; }
        public double MaxPoint { get; set; }
        public double MinPoint { get; set; }
        public double WinPrecentage { get; set; }
        public int NoOfQuestion { get; set; }
        public bool isActive { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}