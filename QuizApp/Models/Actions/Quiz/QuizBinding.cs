using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;
using QuizApp.Models.Input.Quiz;

namespace QuizApp.Models
{
    public class QuizBinding
    {
        #region Quiz
        public List<QuizResult> GetQuiz()
        {
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();
            QuizAppEntities entities = new QuizAppEntities();
            DateTime today = DateTime.Now.Date;
            var data = entities.QuizDatas.Where(x => x.isActive == true && x.StartDate.Value.Year == today.Year && x.StartDate.Value.Month == today.Month && x.StartDate.Value.Day == today.Day).Select(a => new QuizResult()
            {
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,
                QuizBannerImage = ImageSource + a.QuizBannerImage,
                NoOfQuestion = a.NoOfQuestion.Value,
                PlayingDescriptionImg = ImageSource + a.PlayingDescriptionImg,
                StartDate = a.StartDate.Value,
                WinPrecentage = a.WinPrecentage
            }).ToList();

            data.ForEach(a =>
            {
                a.StartDateStr = a.StartDate.ToString("dd MMM hh:mm tt");
                a.isActive = DateTime.Compare(a.StartDate, DateTime.Now) <= 0;
            });

            return data;
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
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();

            QuizQuestionResultMain quizQuestionResultMain = new QuizQuestionResultMain();

            QuizAppEntities entities = new QuizAppEntities();
            var quiz = entities.QuizDatas.Where(a => a.QuizID == quizId).FirstOrDefault();

            quizQuestionResultMain.Questions = entities.QuizQuestions.Where(x => x.QuizID == quizId).Select(a => new QuizQuestionResult()
            {
                QuizID = a.QuizID,
                QuizQuestionID = a.QuizQuestionID,
                CorrectOption = a.CorrectOption,
                ImageUrl = ImageSource + a.ImageUrl,
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

        #region Start Game
        public int StartGame(StartGameBindingModel model)
        {
            QuizAppEntities entities = new QuizAppEntities();
            var player = new QuizPlayer()
            {
                CreatedDate = DateTime.Now,
                IsCompleted = false,
                IsWon = false,
                Language = "",
                PercentageEarn = 0,
                PlayedDate = DateTime.Now,
                PointEarn = 0,
                QuizID = model.QuizId,
                UserID = model.UserId
            };
            entities.QuizPlayers.Add(player);
            entities.SaveChanges();
            return player.PlayerID;
        }
        #endregion

        #region Set Question Answer
        public int SetQuestionAnswer(SetQuestionAnswerBindingModel model)
        {
            QuizAppEntities entities = new QuizAppEntities();
            var quizQuestion = entities.QuizQuestions.Where(a => a.QuizQuestionID == model.QuizQuestionID).FirstOrDefault();

            var IsCorrect = (quizQuestion.CorrectOption == model.SelectedOption);
            var PointEarn = 0;
            if (IsCorrect)
            {
                PointEarn = quizQuestion.QuestionPoint.Value;
            }


            var player = new UserAnswer()
            {
                CreatedDate = DateTime.Now,
                PlayerID = model.PlayerID,
                IsCorrect = IsCorrect,
                PointEarn = PointEarn,
                QuizQuestionID = model.QuizQuestionID,
                SelectedOption = model.SelectedOption,
                TimeTaken = model.TimeTakeninSeconds
            };
            entities.UserAnswers.Add(player);
            entities.SaveChanges();
            return player.ID;
        }
        #endregion

        #region End Game

        public int EndGame(EndGameBindingModel model)
        {
            QuizAppEntities entities = new QuizAppEntities();
            var quizData = entities.QuizDatas.Where(x => x.QuizID == model.QuizID).FirstOrDefault();

            if (quizData != null)
            {
                int minPoint = (int)quizData.MinPoint;

                int maxPoint = (int)quizData.MinPoint;

                int userAnswerTotal = (int)entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.PointEarn);

                int totalGetPointEarned = minPoint + userAnswerTotal;

                //Get percentage earn
                int percentageEarn = (totalGetPointEarned * 100) / maxPoint;

                //Get IsWon
                bool isWon = false;
                if (quizData.WinPrecentage <= percentageEarn)
                {
                    isWon = true;
                }

                //Insert user points 
                UserPoint userPoint = new UserPoint()
                {
                    UserID = model.UserID,
                    TransactionDate = DateTime.Now,
                    PointsEarned = totalGetPointEarned,
                    PointsWithdraw = 0,
                    Description = "Point earnd from quiz id : " + model.QuizID,
                    CreatedDate = DateTime.Now
                };

                entities.UserPoints.Add(userPoint);

                //Update or Insert Userwallet
                var userWallet = entities.UserWallets.Where(x => x.UserID == model.UserID).FirstOrDefault();

                if (userWallet != null)
                {
                    UserWallet wallet = new UserWallet()
                    {
                        CurrentBalance = userWallet.CurrentBalance + totalGetPointEarned,
                        CurrentPoint = userWallet.CurrentPoint + totalGetPointEarned,
                        LastUpdated = DateTime.Now,
                        TotalEarn = userWallet.TotalEarn + totalGetPointEarned,
                        TotalWithdraw = userWallet.TotalWithdraw + totalGetPointEarned,
                        PendingWithdraw = userWallet.PendingWithdraw + totalGetPointEarned
                    };
                    entities.Entry(userWallet).CurrentValues.SetValues(wallet);
                }
                else
                {
                    UserWallet wallet = new UserWallet()
                    {
                        UserID = model.UserID,
                        CurrentBalance = totalGetPointEarned,
                        CurrentPoint = totalGetPointEarned,
                        LastUpdated = DateTime.Now,
                        TotalEarn = totalGetPointEarned,
                        TotalWithdraw = totalGetPointEarned,
                        PendingWithdraw = totalGetPointEarned,
                        CreatedDate = DateTime.Now
                    };
                    entities.UserWallets.Add(wallet);
                }

                //Update QuizPlayer
                var quizPlayer = entities.QuizPlayers.Where(x => x.PlayerID == model.PlayerID).FirstOrDefault();

                if (quizPlayer != null)
                {
                    QuizPlayer player = new QuizPlayer()
                    {
                        QuizID = model.QuizID,
                        IsCompleted = true,
                        IsWon = isWon,
                        PointEarn = totalGetPointEarned,
                        PlayedDate = DateTime.Now,
                        PercentageEarn = percentageEarn,
                        Language = "English"
                    };

                    entities.Entry(quizPlayer).CurrentValues.SetValues(player);
                }
                return entities.SaveChanges();
            }
            else
            {
                return 0;
            }
        }

        #endregion
    }
}