using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class QuizBinding
    {
        #region Quiz
        public List<QuizResult> GetQuiz()
        {
            QuizAppEntities entities = new QuizAppEntities();
            var data = entities.QuizDatas.Where(x => x.isActive == true).Select(a => new QuizResult()
            {
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,
                QuizBannerImage = a.QuizBannerImage,
                isActive = a.isActive.Value,
                NoOfQuestion = a.NoOfQuestion.Value,
                PlayingDescriptionImg = a.PlayingDescriptionImg,
                StartDate = a.StartDate.Value,
                StartDateStr = a.StartDate.Value.ToString("dd MMM, yyyy"),
                WinPrecentage = a.WinPrecentage
            });

            return data.Where(x => DateTime.Compare(x.StartDate, DateTime.Now) < 0).ToList();
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
        #endregion

        #region Quiz Questions

        public QuizQuestionResultMain GetQuizQuestions(Guid quizId, string UserId)
        {
            QuizQuestionResultMain quizQuestionResultMain = new QuizQuestionResultMain();

            QuizAppEntities entities = new QuizAppEntities();
            var quiz = entities.QuizDatas.Where(a => a.QuizID == quizId).FirstOrDefault();

            quizQuestionResultMain.Questions = entities.QuizQuestions.Where(x => x.QuizID == quizId).Select(a => new QuizQuestionResult()
            {
                QuizID = a.QuizID,
                QuizQuestionID = a.QuizQuestionID,
                CorrectOption = a.CorrectOption,
                ImageUrl = a.ImageUrl,
                MaxTime = (int)a.MaxTime,
                Option1 = a.Option1,
                Option2 = a.Option2,
                Option3 = a.Option3,
                Option4 = a.Option4,
                Question = a.Question,
                QuestionPoint = (int)a.QuestionPoint,

            }).OrderBy(r => Guid.NewGuid()).Take(quiz.NoOfQuestion.Value).ToList();

            var existingData = entities.QuizPlayers.Where(a => a.QuizID == quizId && a.UserID == UserId).OrderByDescending(a => a.PlayedDate).FirstOrDefault();

            if (existingData != null)
            {
                quizQuestionResultMain.AlreadyPlayed = true;
                quizQuestionResultMain.IsCompleted = existingData.IsCompleted.Value;
                quizQuestionResultMain.IsWon = existingData.IsWon.Value;
                quizQuestionResultMain.PlayedDate = existingData.PlayedDate.Value.ToString("dd MMM, yyyy");
                quizQuestionResultMain.PlayerID = existingData.PlayerID;
                quizQuestionResultMain.PointEarn = existingData.PointEarn.Value;
            }
            else
            {
                quizQuestionResultMain.AlreadyPlayed = false;
            }
            return quizQuestionResultMain;
        }

        #endregion
    }
}