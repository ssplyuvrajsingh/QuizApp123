using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;
using QuizApp.Models.Input.Quiz;
using QuizApp.Models.Output.QuizData;

namespace QuizApp.Models
{
    public class QuizBinding
    {
        #region Database Entities Declaration
        QuizAppEntities entities = new QuizAppEntities();
        #endregion

        #region Quiz
        public List<QuizResult> GetQuiz()
        {
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();
            var data = entities.QuizDatas.Where(x => x.isActive == true).Select(a => new QuizResult()
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
            var quiz = entities.QuizDatas.Where(a => a.QuizID == quizId).FirstOrDefault();
            if (quiz != null)
            {
                List<QuizQuestionResult> lstQuizQuestionResult = new List<QuizQuestionResult>();
                var QuizQuestions = entities.QuizQuestions.Where(x => x.QuizID == quizId).OrderBy(r => Guid.NewGuid()).Take(quiz.NoOfQuestion.Value).ToList();
                foreach (var a in QuizQuestions)
                {
                    var quizQuestionResult = new QuizQuestionResult();
                    quizQuestionResult.QuizQuestionID = a.QuizQuestionID;
                    quizQuestionResult.QuizID = a.QuizID;
                    quizQuestionResult.CorrectOption = a.CorrectOption;
                    quizQuestionResult.ImageUrl = ImageSource + a.ImageUrl;
                    quizQuestionResult.MaxTime = (int)a.MaxTime;
                    quizQuestionResult.Options = new string[] { a.Option1, a.Option2, a.Option3, a.Option4 };
                    quizQuestionResult.Question = a.Question;
                    quizQuestionResult.QuestionPoint = (int)a.QuestionPoint;
                    lstQuizQuestionResult.Add(quizQuestionResult);
                }
                quizQuestionResultMain.Questions = lstQuizQuestionResult; 
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
            return null;
        }

        #endregion

        #region Get Quize PlayerID

        #endregion

        #region SetQuizPlayer
        public int SetQuizPlayermodle(QuizPlayer model)
        {
            var data = entities.QuizPlayers.Add(model);
            entities.SaveChanges();
            return data.PlayerID;
        }
        #endregion

        #region Set Quize Player
        public QuizPlayerResult SetQuizePlayer(QuizPlayerResult model)
        {

            int PlayerID = SetQuizPlayermodle(new QuizPlayer()
            {
                UserID = model.UserID,
                QuizID = model.QuizID,
                PlayedDate = model.PlayedDate,
                Language = "English",
                CreatedDate = DateTime.Now
            });

            foreach (var item in model.UserAnswer)
            {
                var quizQuestion = entities.QuizQuestions.Where(a => a.QuizQuestionID == item.QuizQuestionID).FirstOrDefault();
                item.PointEarn = 0;
                if (quizQuestion.CorrectOption == item.SelectedOption)
                {
                    item.PointEarn = quizQuestion.QuestionPoint.Value;
                }
                entities.UserAnswers.Add(new UserAnswer()
                {
                    PlayerID = PlayerID,
                    QuizQuestionID = item.QuizQuestionID,
                    SelectedOption = item.SelectedOption,
                    TimeTaken = item.TimeTaken,
                    IsCorrect = item.IsCorrect,
                    PointEarn = item.PointEarn,
                    CreatedDate = DateTime.Now
                });
            }
            entities.SaveChanges();
            model.PointEarn = Convert.ToInt32(entities.UserAnswers.Where(x => x.PlayerID == PlayerID).Sum(x => x.PointEarn));
            model.PercentageEarn = model.PointEarn * 100 / model.TotalPoint;
            int WinPrecentage = entities.QuizDatas.Where(x => x.QuizID == model.QuizID).Select(s => s.WinPrecentage).FirstOrDefault();
            if(model.PercentageEarn>=WinPrecentage)
            {
                model.IsWon = true;
            }
            else
            {
                model.IsWon = false;
            }
           
            int UserAnswerCount = entities.UserAnswers.Where(x => x.PlayerID == PlayerID).Count();
            if(UserAnswerCount == entities.QuizQuestions.Where(x=>x.QuizID == model.QuizID).Count())
            {
                model.IsCompleted = true;
            }

            var data = entities.QuizPlayers.Where(x => x.PlayerID == PlayerID).FirstOrDefault();
            if (data!=null)
            {
                data.IsCompleted = model.IsCompleted;
                data.IsWon = model.IsWon;
                data.PointEarn = entities.UserAnswers.Where(a => a.PlayerID == PlayerID).Sum(s => s.PointEarn);
                data.PercentageEarn = model.PercentageEarn;
            }

            var FinalResult = entities.QuizPlayers.Where(x => x.PlayerID == PlayerID).FirstOrDefault();
            int second = entities.UserAnswers.Where(x => x.PlayerID == PlayerID).Sum(s => s.TimeTaken);
            TimeSpan time = TimeSpan.FromSeconds(second);
            string str = time.ToString(@"hh\:mm\:ss\:fff");
            return new QuizPlayerResult()
            {
                UserID = FinalResult.UserID,
                QuizID = FinalResult.QuizID,
                IsCompleted = Convert.ToBoolean(FinalResult.IsCompleted),
                IsWon = Convert.ToBoolean(FinalResult.IsWon),
                PointEarn = Convert.ToInt32(FinalResult.PointEarn),
                PercentageEarn = Convert.ToInt32(FinalResult.PercentageEarn),
                TotalTimeTaken = str,
                Language=FinalResult.Language
            };
        }
        #endregion

        #region Start Game
        public int StartGame(StartGameBindingModel model)
        {
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
        //public int SetQuestionAnswer(SetQuestionAnswerBindingModel model)
        //{
        //    var quizQuestion = entities.QuizQuestions.Where(a => a.QuizQuestionID == model.QuizQuestionID).FirstOrDefault();

        //    var IsCorrect = (quizQuestion.CorrectOption == model.SelectedOption);
        //    var PointEarn = 0;
        //    if (IsCorrect)
        //    {
        //        PointEarn = quizQuestion.QuestionPoint.Value;
        //    }


        //    var player = new UserAnswer()
        //    {
        //        CreatedDate = DateTime.Now,
        //        PlayerID = model.PlayerID,
        //        IsCorrect = IsCorrect,
        //        PointEarn = PointEarn,
        //        QuizQuestionID = model.QuizQuestionID,
        //        SelectedOption = model.SelectedOption,
        //        TimeTaken = model.TimeTakeninSeconds
        //    };
        //    entities.UserAnswers.Add(player);
        //    entities.SaveChanges();
        //    return player.ID;
        //}
        #endregion

        #region End Game

        public EndGameResult EndGame(EndGameBindingModel model)
        {
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
                        PendingWithdraw = userWallet.PendingWithdraw + totalGetPointEarned,
                        CreatedDate = userWallet.CreatedDate,
                        WalletID = userWallet.WalletID,
                        UserID = userWallet.UserID
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
                        CreatedDate = DateTime.Now,
                    };
                    entities.UserWallets.Add(wallet);
                }

                //Update QuizPlayer
                var quizPlayer = entities.QuizPlayers.Where(x => x.PlayerID == model.PlayerID).FirstOrDefault();

                if (quizPlayer != null)
                {
                    QuizPlayer player = new QuizPlayer()
                    {
                        PlayerID = quizPlayer.PlayerID,
                        QuizID = model.QuizID,
                        IsCompleted = true,
                        IsWon = isWon,
                        PointEarn = totalGetPointEarned,
                        PlayedDate = DateTime.Now,
                        PercentageEarn = percentageEarn,
                        Language = "English",
                        UserID = quizPlayer.UserID,
                        CreatedDate = quizPlayer.CreatedDate
                    };

                    entities.Entry(quizPlayer).CurrentValues.SetValues(player);
                }
                entities.SaveChanges();

                return new EndGameResult()
                {
                    PercentageEarn = percentageEarn,
                    PointsEarned = totalGetPointEarned,
                    IsWon = isWon,
                    TimeTakeninSeconds = entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.TimeTaken)
                };
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region GetScoreByQuiz

        public GetScoreResult GetScoreByQuiz(GetScoreByQuiz model)
        {
            var quizPlayer = entities.QuizPlayers.Where(x => x.UserID == model.UserID && x.QuizID == model.QuizID);
            if (quizPlayer != null)
            {
                int playerBestScore = (int)quizPlayer.Max(x => x.PointEarn);
                return new GetScoreResult()
                {
                    YourBestScore = playerBestScore,
                    OverallScore = (int)entities.QuizDatas.Where(x => x.QuizID == model.QuizID).FirstOrDefault().MaxPoint
                };
            }
            else
            {
                return new GetScoreResult()
                {
                    YourBestScore = 0,
                    OverallScore = (int)entities.QuizDatas.Where(x => x.QuizID == model.QuizID).FirstOrDefault().MaxPoint
                };
            }
        }

        #endregion
    }
}