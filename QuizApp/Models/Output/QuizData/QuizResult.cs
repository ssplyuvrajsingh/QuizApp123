using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizResult
    {
        public Guid QuizID { get; set; }
        public string QuizTitle { get; set; }
        public string PlayingDescriptionImg { get; set; }
        public DateTime StartDate { get; set; }
        public string StartDateStr { get; set; }
        public string QuizBannerImage { get; set; }
        public int MaxPoint { get; set; }
        public int MinPoint { get; set; }
        public int WinPrecentage { get; set; }
        public int NoOfQuestion { get; set; }
        public bool isActive { get; set; }
    }

}