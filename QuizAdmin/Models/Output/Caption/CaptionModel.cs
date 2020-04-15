using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class CaptionModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
    }
}