using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizIDModel
    {
        public Guid QuizId { get; set; }
        public string UserId { get; set; }
    }
    public class TopResult
    {
        public string Name { get; set; }
        public int Score { get; set; }
        public string Time { get; set; }
        public bool status { get; set; }
    }
}