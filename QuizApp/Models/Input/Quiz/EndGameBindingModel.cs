using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input.Quiz
{
    public class EndGameBindingModel
    {
        [Required]
        public Guid QuizID { get; set; }

        [Required]
        public string UserID { get; set; }

        [Required]
        public int PlayerID { get; set; }
    }

}