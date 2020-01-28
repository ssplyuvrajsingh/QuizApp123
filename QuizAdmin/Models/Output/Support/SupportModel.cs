using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class SupportModel
    {
        public string UserName { get; set; }
        public string PhoneNumber { get; set; }
        public string UserQuery { get; set; }
    }

    public class UserSupportModel
    {

        //[Required(ErrorMessage = "UserId Required.")]
        //public string UserId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Display(Name = "Mobile Number:")]
        [Required(ErrorMessage = "Mobile Number is required.")]
        [RegularExpression(@"^([0-9]{10})$", ErrorMessage = "Invalid Mobile Number.")]
        public string PhoneNumber { get; set; }
        [Required]
        public string UserQuery { get; set; }
    }
}