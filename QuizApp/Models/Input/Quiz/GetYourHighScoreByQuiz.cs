using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input.Quiz
{
    public class GetScoreByQuiz
    {
        [Required]
        public string UserID { get; set; }

        [Required]
        public Guid QuizID { get; set; }
    }
}