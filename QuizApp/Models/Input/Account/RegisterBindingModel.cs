using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class RegisterBindingModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Password { get; set; }
        public string UsedReferalCode { get; set; }
        public string DeviceID { get; set; }
        public string Platform { get; set; }
        public string NotificationKey { get; set; }
        public string IP { get; set; }

        //Other
        public string UserId { get; set; }
    }
}