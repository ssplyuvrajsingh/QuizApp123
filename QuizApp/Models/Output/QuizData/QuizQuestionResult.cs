using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Output.QuizData
{
    public class QuizQuestionResult
    {
        public int QuizQuestionID { get; set; }
        public Guid QuizID { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectOption { get; set; }
        public int MaxTime { get; set; }
        public int QuestionPoint { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}