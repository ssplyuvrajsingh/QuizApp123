using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class LevelWiseModel
    {
        public int Level { get; set; }
        public string UserId { get; set; }
    }

    public class LevelWiseActiveUsers
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }
}