﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class RefreshTokenBindingModel
    {
        [Required]
        [Display(Name = "RefreshToken")]
        public string RefreshToken { get; set; }
    }
}