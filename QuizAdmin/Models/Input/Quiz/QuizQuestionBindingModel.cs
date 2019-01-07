using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models.Input.Quiz
{
    public class QuizQuestionBindingModel
    {
        [Required]
        [Display(Name = "Quiz Id")]
        public Guid QuizID { get; set; }

        [Required]
        [Display(Name = "Question")]
        public string Question { get; set; }

        [Required]
        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }

        [Required]
        [Display(Name = "Option 1")]
        public string Option1 { get; set; }
        [Required]
        [Display(Name = "Option 2")]
        public string Option2 { get; set; }

        [Required]
        [Display(Name = "Option 3")]
        public string Option3 { get; set; }

        [Required]
        [Display(Name = "Option 4")]
        public string Option4 { get; set; }

        [Required]
        [Display(Name = "Correct Option")]
        public string CorrectOption { get; set; }

        [Required]
        [Display(Name = "Max Time")]
        public int MaxTime { get; set; }

        [Required]
        [Display(Name = "Question Point")]
        public int QuestionPoint { get; set; }
    }
}