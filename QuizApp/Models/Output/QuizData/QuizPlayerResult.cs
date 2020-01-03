using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizPlayerResult
    {
        public string UserID { get; set; }
        public System.Guid QuizID { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsWon { get; set; }
        public int PointEarn { get; set; }
        public System.DateTime PlayedDate { get; set; }
        public int PercentageEarn { get; set; }
        public string Language { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public int TotalPoint { get; set; }
        public List<UserAnswerClass> UserAnswer { get; set; }
        public string TotalTimeTaken { get; set; }
    }
    public class UserAnswerClass
    {
        public int PlayerID { get; set; }
        public int QuizQuestionID { get; set; }
        public string SelectedOption { get; set; }
        public int TimeTaken { get; set; }
        public bool IsCorrect { get; set; }
        public Nullable<int> PointEarn { get; set; }
    }
}