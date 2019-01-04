using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class SetQuestionAnswerBindingModel
    {
        [Required]
        public int PlayerID { get; set; }
        [Required]
        public int QuizQuestionID { get; set; }
        [Required]
        public string SelectedOption { get; set; }
        [Required]
        public int TimeTakeninSeconds { get; set; }
        [Required]
        public string UserID { get; set; }
    }
}