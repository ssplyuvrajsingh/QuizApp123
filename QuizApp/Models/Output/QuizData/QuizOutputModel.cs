using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizApp.Models.Entities;

namespace QuizApp.Models.Output.QuizData
{
    public class QuizOutputModel
    {
        public List<Models.Entities.QuizData> quizDatas { get; set; }
    }
}