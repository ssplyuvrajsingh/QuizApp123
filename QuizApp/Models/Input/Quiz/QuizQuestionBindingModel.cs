using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input.Quiz
{
    public class QuizQuestionBindingModel
    {
        [Required]
        [Display(Name = "Quiz Id")]
        public string QuizId { get; set; }
    }
}