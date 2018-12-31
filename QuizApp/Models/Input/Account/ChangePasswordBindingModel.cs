using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class ChangePasswordBindingModel
    {
        [Required]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]        
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
       
    }
}