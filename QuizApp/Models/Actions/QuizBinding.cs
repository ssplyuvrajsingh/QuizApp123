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
        public ResultClass GetQuiz()
        {
            ResultClass result = new ResultClass();
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var quiz = entities.QuizDatas.Where(x => x.StartDate == DateTime.Now.Date && x.isActive == true).ToList();
                    
                    if(quiz != null)
                    {
                        result.Result = true;
                        result.Message = "Get Quiz successfully";
                        result.Data = quiz;
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "Empty quiz list";
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
                        result.Message = "Get Quiz question successfully";
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
            catch(Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}