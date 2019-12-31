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
    }
}