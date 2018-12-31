using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using QuizApp.Models.Output.QuizData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Actions.Quiz
{
    public class QuizBinding
    {
        public List<QuizResult> GetQuiz()
        {
            QuizAppEntities entities = new QuizAppEntities();
            return entities.QuizDatas.Where(x => Convert.ToDateTime(x.StartDate).Date == DateTime.Now.Date && x.isActive == true).Select(a => new QuizResult()
            {
                isActive = (bool)a.isActive,
                CreatedDate = a.CreatedDate,
                MaxPoint = (double)a.MaxPoint,
                MinPoint = (double)a.MinPoint,
                NoOfQuestion = (int)a.NoOfQuestion,
                PlayingDescriptionImg = a.PlayingDescriptionImg,
                QuizBannerImage = a.QuizBannerImage,
                QuizDate = (DateTime)a.QuizDate,
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,
                StartDate = (DateTime)a.StartDate,
                WinPrecentage = a.WinPrecentage
            }).ToList();
        }

        public List<QuizQuestionResult> GetQuizQuestions(string quizId)
        {
            QuizAppEntities entities = new QuizAppEntities();
            return entities.QuizQuestions.Where(x => x.QuizID.ToString() == quizId).Select(a => new QuizQuestionResult()
            {
                QuizID = a.QuizID,
                CorrectOption = a.CorrectOption,
                CreatedDate = a.CreatedDate,
                ImageUrl = a.ImageUrl,
                MaxTime = (int)a.MaxTime,
                Option1 = a.Option1,
                Option2 = a.Option2,
                Option3 = a.Option3,
                Option4 = a.Option4,
                Question = a.Question,
                QuestionPoint = (int)a.QuestionPoint,
                QuizQuestionID = a.QuizQuestionID
            }).ToList();
        }
    }
}