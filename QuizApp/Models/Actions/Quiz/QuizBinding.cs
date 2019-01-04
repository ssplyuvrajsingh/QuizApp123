using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using QuizApp.Models.Input.Quiz;
using QuizApp.Models.Output.QuizData;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Actions.Quiz
{
    public class QuizBinding
    {
        public List<QuizResult> GetQuiz()
        {
            QuizAppEntities entities = new QuizAppEntities();
            var data =  entities.QuizDatas.Where(x=> x.isActive == true).Select(a => new QuizResult()
            {
                isActive = a.isActive.Value,
                CreatedDate = a.CreatedDate,
                MaxPoint = a.MaxPoint.Value,
                MinPoint = a.MinPoint.Value,
                NoOfQuestion = a.NoOfQuestion.Value,
                PlayingDescriptionImg = a.PlayingDescriptionImg,
                QuizBannerImage = a.QuizBannerImage,
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,
                StartDate = a.StartDate.Value,
                WinPrecentage = a.WinPrecentage
            }).ToList();

            var result = data.Where(x => DateTime.Compare(x.StartDate.Date, DateTime.Now.Date) == 0).ToList();

            return result;
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

        public int SetQuiz(QuizBindingModel model)
        {
            QuizAppEntities entities = new QuizAppEntities();

            QuizData quizData = new QuizData()
            {
                CreatedDate = DateTime.Now,
                isActive = true,
                MaxPoint = model.MaxPoint,
                MinPoint = model.MinPoint,
                NoOfQuestion = model.NoOfQuestion,
                PlayingDescriptionImg = model.PlayingDescriptionImg,
                QuizBannerImage = model.QuizBannerImage,
                StartDate = model.StartDate,
                QuizID = Guid.NewGuid(),
                QuizTitle = model.QuizTitle,
                WinPrecentage = model.WinPrecentage
            };

            entities.QuizDatas.Add(quizData);
            return entities.SaveChanges();
        }
    }
}