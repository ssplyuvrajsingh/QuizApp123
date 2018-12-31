using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Input
{
    public class QuizBindingModel
    {
        [Required]
        public string QuizTitle { get; set; }
        [Required]
        public string PlayingDescriptionImg { get; set; }
        [Required]
        public DateTime QuizDate { get; set; }
        public string QuizBannerImage { get; set; }
        public float MaxPoint { get; set; }
        public float MinPoint { get; set; }
        public float WinPrecentage { get; set; }
        public int NoOfQuestion { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
    }

    public class QuizQuestionBinding
    {
        [Required]
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        [Required]
        public string Option1 { get; set; }
        [Required]
        public string Option2 { get; set; }
        [Required]
        public string Option3 { get; set; }
        [Required]
        public string Option4 { get; set; }
        [Required]
        public string CorrectOption { get; set; }
        public int MaxTime { get; set; }
        public int QuestionPoint { get; set; }
    }
}