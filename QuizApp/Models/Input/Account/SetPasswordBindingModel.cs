using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class SetPasswordBindingModel
    {
        [Required]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        public string ciphertoken { get; set; }
    }
}