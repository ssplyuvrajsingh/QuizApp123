using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input
{
    public class QuizBindingModel
    {
        public string QuizId { get; set; }
        public string QuizTitle { get; set; }
        public string PlayingDescriptionImg { get; set; }
        public DateTime QuizDate { get; set; }
        public string QuizBannerImage { get; set; }
        public float MaxPoint { get; set; }
        public float MinPoint { get; set; }
        public float WinPrecentage { get; set; }
        public int NoOfQuestion { get; set; }
    }

    public class QuizQuestionBinding
    {
        public string QuizQuestionId { get; set; }
        public string QuizId { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectOption { get; set; }
        public TimeSpan MaxTime { get; set; }
        public int QuestionPoint { get; set; }
    }
}