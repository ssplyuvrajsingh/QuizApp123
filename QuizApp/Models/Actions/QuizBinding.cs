using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Actions
{
    public class QuizBinding
    {
        public List<QuizData> GetQuiz()
        {
            QuizAppEntities entities = new QuizAppEntities();
            return entities.QuizDatas.Where(x => x.StartDate == DateTime.Now.Date && x.isActive == true).ToList().Select(a => new QuizData()
            {
                isActive = true,
            }).ToList();
        }

        public ResultClass GetQuizQuestions(string quizId)
        {
            ResultClass result = new ResultClass();
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var quizQuestions = entities.QuizQuestions.Where(x => x.QuizID.ToString() == quizId).ToList();

                    if (quizQuestions != null)
                    {
                        result.Result = true;
                        result.Message = "Get quiz question successfully";
                        result.Data = quizQuestions;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "Empty quiz question list";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}