using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizQuestionResultMain
    {
        public List<QuizQuestionResult> Questions { get; set; }
        public bool AlreadyPlayed { get; set; }
        public int PlayerID { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsWon { get; set; }
        public int PointEarn { get; set; }
        public string PlayedDate { get; set; }
    }

    public class QuizQuestionResult
    {
        public int QuizQuestionID { get; set; }
        public QuizQuestionSet QuizQuestionSet { get; set; }
    }

    public class QuizQuestionSet
    {
        public Guid QuizID { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public QuestionOptions Options { get; set; }
        public string CorrectOption { get; set; }
        public int MaxTime { get; set; }
        public int QuestionPoint { get; set; }
    }

    public class QuestionOptions
    {
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
    }
}