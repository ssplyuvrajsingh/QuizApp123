using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Entities
{
    public class QuizBindingModel
    {
        [Required]
        public string QuizTitle { get; set; }
        [Required]
        public string PlayingDescriptionImg { get; set; }
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public string QuizBannerImage { get; set; }
        [Required]
        public int MaxPoint { get; set; }
        [Required]
        public int MinPoint { get; set; }
        [Required]
        public int WinPrecentage { get; set; }
        [Required]
        public int NoOfQuestion { get; set; }
    }
}