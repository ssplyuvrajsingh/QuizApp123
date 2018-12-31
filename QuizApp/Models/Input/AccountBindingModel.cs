using System;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models.Input
{
    public class RegisterBindingModel
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
        public string UserId { get; set; }
        public string ParentIDs { get; set; }
        public string DeviceID { get; set; }
        public string Platform { get; set; }
        public DateTime LastActiveDate { get; set; }
        public string ReferalCode { get; set; }
        public string UsedReferalCode { get; set; }
        public string NotificationKey { get; set; }
        public bool isActive { get; set; }
        public bool isBlocked { get; set; }
        public string Password { get; set; }
        public string IP { get; set; }
    }

    public class LoginBindingModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Password")]
        public string Password { get; set; }

    }

    public class SetPasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }
        
        public string PhoneNumber { get; set; }
        
        public string OTP { get; set; }
    }

    public class ChangePasswordBindingModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        public string PhoneNumber { get; set; }

        //[DataType(DataType.Password)]
        //[Display(Name = "Confirm new password")]
        //[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        //public string ConfirmPassword { get; set; }
    }

    public class ForgotPasswordBindingModel
    {
        [Required]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        
        public string OTP { get; set; }
    }
}