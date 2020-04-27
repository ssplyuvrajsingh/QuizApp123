using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace QuizApp.Models
{
    public class CaptionModel
    {
        public string Title { get; set; }
        public string Url { get; set; }
    }
    public class RssPagingModel
    {
        public string Url { get; set; }
        public int PageNo { get; set; }
        public string Title { get; set; }
    }
}