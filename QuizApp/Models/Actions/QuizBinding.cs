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
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var quiz = entities.QuizDatas.Where(x => x.StartDate == DateTime.Now.Date && x.isActive == true).ToList();
                    
                    return quiz;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public List<QuizQuestion> GetQuizQuestions(string quizId)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var quizQuestions = entities.QuizQuestions.Where(x => x.QuizID.ToString() == quizId).ToList();

                    return quizQuestions;
                }
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}