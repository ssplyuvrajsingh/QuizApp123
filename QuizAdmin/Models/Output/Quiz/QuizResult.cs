using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models.Output.Quiz
{
    public class QuizResult
    {
        public string QuizTitle { get; set; }

        public string PlayingDescriptionImg { get; set; }

        public string QuizBannerImage { get; set; }

        public int MaxPoint { get; set; }

        public int MinPoint { get; set; }

        public int WinPrecentage { get; set; }

        public int NoOfQuestion { get; set; }

        public bool isActive { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}