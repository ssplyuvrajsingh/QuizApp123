﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizQuestionBindingModel
    {
        [Required]
        public Guid QuizId { get; set; }

        [Required]
        public string UserId { get; set; }
        public string ciphertoken { get; set; }
    }
}