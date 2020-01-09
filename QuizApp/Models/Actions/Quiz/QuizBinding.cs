using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Configuration;
using QuizApp.Models.Input.Quiz;
using QuizApp.Models.Output.QuizData;
using Newtonsoft.Json;
using System.IO;

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

            //var data = (from QuizDatas in entities.QuizDatas
            //            join QuizPlayer in entities.QuizPlayers on QuizDatas.QuizID equals QuizPlayer.QuizID
            //            join Users in entities.Users on QuizPlayer.UserID equals Users.UserID

            //            select new QuizResult()
            //            {
            //                QuizID = QuizDatas.QuizID,
            //                QuizTitle = QuizDatas.QuizTitle,
            //                QuizBannerImage = ImageSource + QuizDatas.QuizBannerImage,
            //                NoOfQuestion = QuizDatas.NoOfQuestion.Value,
            //                PlayingDescriptionImg = ImageSource + QuizDatas.PlayingDescriptionImg,
            //                StartDate = QuizDatas.StartDate.Value,
            //                WinPrecentage = QuizDatas.WinPrecentage
            //            }).ToList();

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
                    quizQuestionResult.ImageUrl = ImageSource + a.ImageUrl;
                    quizQuestionResult.MaxTime = (int)a.MaxTime;
                    quizQuestionResult.Options = new string[] { a.Option1, a.Option2, a.Option3, a.Option4 };
                    quizQuestionResult.Question = a.Question;
                    quizQuestionResult.QuestionPoint = (int)a.QuestionPoint;
                    lstQuizQuestionResult.Add(quizQuestionResult);
                }
                quizQuestionResultMain.Questions = lstQuizQuestionResult;
                var existingData = entities.QuizPlayers.Where(a => a.QuizID == quizId && a.UserID == UserId).OrderByDescending(a => a.PlayedDate).FirstOrDefault();
                //if (existingData != null)
                //{
                //    quizQuestionResultMain.AlreadyPlayed = true;
                //    quizQuestionResultMain.IsCompleted = existingData.IsCompleted.Value;
                //    quizQuestionResultMain.IsWon = existingData.IsWon.Value;
                //    quizQuestionResultMain.PlayedDate = existingData.PlayedDate.Value.ToString("dd MMM, yyyy");
                //    quizQuestionResultMain.PlayerID = existingData.PlayerID;
                //    quizQuestionResultMain.PointEarn = existingData.PointEarn.Value;
                //}
                //else
                //{
                //    quizQuestionResultMain.AlreadyPlayed = false;
                //}
                return quizQuestionResultMain;
            }
            return null;
        }

        #endregion

        #region SetQuizPlayer
        public int SetQuizPlayermodle(QuizPlayer model)
        {
            var data = entities.QuizPlayers.Add(model);
            entities.SaveChanges();
            return data.PlayerID;
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

        #region Set Quize Player
        public EndGameResult SetQuizePlayer(QuizPlayerResult model)
        {
            #region Set User Answer
            foreach (var item in model.UserAnswer)
            {
                var quizQuestion = entities.QuizQuestions.Where(a => a.QuizQuestionID == item.QuizQuestionID).FirstOrDefault();
                item.PointEarn = 0;
                if (quizQuestion.CorrectOption == item.SelectedOption)
                {
                    item.PointEarn = quizQuestion.QuestionPoint.Value;
                    item.IsCorrect = true;
                }
                entities.UserAnswers.Add(new UserAnswer()
                {
                    PlayerID = model.PlayerId,
                    QuizQuestionID = item.QuizQuestionID,
                    SelectedOption = item.SelectedOption,
                    TimeTaken = item.TimeTaken,
                    IsCorrect = item.IsCorrect,
                    PointEarn = item.PointEarn,
                    CreatedDate = DateTime.Now
                });
            }
            entities.SaveChanges();
            #endregion
            return EndGame(new EndGameBindingModel()
            {
                PlayerID = model.PlayerId,
                QuizID = model.QuizID,
                UserID = model.UserID
            });
        }
        #endregion

        #region End Game

        public EndGameResult EndGame(EndGameBindingModel model)
        {
            var quizData = entities.QuizDatas.Where(x => x.QuizID == model.QuizID).FirstOrDefault();

            if (quizData != null)
            {

                int userAnswerTotal = (int)entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.PointEarn);



                //Get percentage earn
                int percentageEarn = (userAnswerTotal * 100) / (int)entities.QuizQuestions.Where(x => x.QuizID == model.QuizID).Sum(x => x.QuestionPoint);

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
                    PointsEarned = userAnswerTotal,
                    PointsWithdraw = 0,
                    Description = "Point earnd from quiz id : " + model.QuizID,
                    CreatedDate = DateTime.Now
                };
                entities.UserPoints.Add(userPoint);

                ////Update or Insert Userwallet
                //var userWallet = entities.UserWallets.Where(x => x.UserID == model.UserID).FirstOrDefault();

                //if (userWallet != null)
                //{
                //    UserWallet wallet = new UserWallet()
                //    {
                //        CurrentBalance = userWallet.CurrentBalance + userAnswerTotal,
                //        CurrentPoint = userWallet.CurrentPoint + userAnswerTotal,
                //        LastUpdated = DateTime.Now,
                //        TotalEarn = userWallet.TotalEarn + userAnswerTotal,
                //        TotalWithdraw = userWallet.TotalWithdraw + userAnswerTotal,
                //        PendingWithdraw = userWallet.PendingWithdraw + userAnswerTotal,
                //        CreatedDate = userWallet.CreatedDate,
                //        WalletID = userWallet.WalletID,
                //        UserID = userWallet.UserID
                //    };
                //    entities.Entry(userWallet).CurrentValues.SetValues(wallet);
                //}
                //else
                //{
                //    UserWallet wallet = new UserWallet()
                //    {
                //        UserID = model.UserID,
                //        CurrentBalance = userAnswerTotal,
                //        CurrentPoint = userAnswerTotal,
                //        LastUpdated = DateTime.Now,
                //        TotalEarn = userAnswerTotal,
                //        TotalWithdraw = userAnswerTotal,
                //        PendingWithdraw = userAnswerTotal,
                //        CreatedDate = DateTime.Now,
                //    };
                //    entities.UserWallets.Add(wallet);
                //}

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
                        PointEarn = userAnswerTotal,
                        PlayedDate = DateTime.Now,
                        PercentageEarn = percentageEarn,
                        Language = "English",
                        UserID = quizPlayer.UserID,
                        CreatedDate = quizPlayer.CreatedDate
                    };

                    entities.Entry(quizPlayer).CurrentValues.SetValues(player);
                }
                entities.SaveChanges();
                int Second = entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.TimeTaken);
                TimeSpan time = TimeSpan.FromSeconds(Second);
                string str = time.ToString(@"mm\:ss");
                return new EndGameResult()
                {
                    PercentageEarn = percentageEarn,
                    PointsEarned = userAnswerTotal,
                    IsWon = isWon,
                    TimeTakeninSeconds = str
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

        #region Get Wallet Information
        public UserWalletModel GetWalletInfo(string UserId)
        {
            UserWalletModel userWallet = new UserWalletModel();
            var data = entities.Users.Where(x => x.UserID == UserId).FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            var transactions = entities.Transactions.Where(x => x.UserID == UserId).ToList();
            foreach (var a in transactions)
            {
                var Tran = new TransactionModel();
                Tran.transactionDateTime = string.Format("{0: dd MMMM }", a.transactionDateTime) + " " + string.Format("{0:hh:mm tt}", a.transactionDateTime);
                Tran.amount = (int)a.amount;
                Tran.paymentStatus = a.paymentStatus;
                transactionModels.Add(Tran);
            }
            userWallet.TransactionModels = transactionModels;
            userWallet.CurrentBalance = data.CurrentBalance != null ? (double)data.CurrentBalance : 0;
            userWallet.MothlyIncome = data.MothlyIncome != null ? (double)data.MothlyIncome : 0;
            userWallet.TotalWithdraw = data.TotalWithdraw != null ? (double)data.TotalWithdraw : 0;
            userWallet.TotalPoins = (int)data.CurrentPoint;
            return userWallet;
        }
        #endregion

        #region Get Level Base Earning Amount
        public LevelEarningModelMaster GetLevelBaseEarningAmount(string Users)
        {
            var data = entities.LevelEarnings.Where(x => x.UserID == Users).FirstOrDefault();
            if (data != null)
            {
                return new LevelEarningModelMaster()
                {
                    Level1ActiveUsers = data.Level1Users != null ? data.Level1Users : 0,
                    Level1Amount = data.Level1 != null ? data.Level1Users : 0,

                    Level2ActiveUsers = data.Level2Users != null ? data.Level2Users : 0,
                    Level2Amount = data.Level2 != null ? data.Level2Users : 0,

                    Level3ActiveUsers = data.Level3Users != null ? data.Level3Users : 0,
                    Level3Amount = data.Level3 != null ? data.Level3Users : 0,

                    Level4ActiveUsers = data.Level4Users != null ? data.Level4Users : 0,
                    Level4Amount = data.Level4 != null ? data.Level4Users : 0,

                    Level5ActiveUsers = data.Level5Users != null ? data.Level5Users : 0,
                    Level5Amount = data.Level5 != null ? data.Level5Users : 0,

                    Level6ActiveUsers = data.Level6Users != null ? data.Level6Users : 0,
                    Level6Amount = data.Level6 != null ? data.Level6Users : 0,

                    Level7ActiveUsers = data.Level7Users != null ? data.Level7Users : 0,
                    Level7Amount = data.Level7 != null ? data.Level7Users : 0,

                    Level8ActiveUsers = data.Level8Users != null ? data.Level8Users : 0,
                    Level8Amount = data.Level8 != null ? data.Level8Users : 0,

                    Level9ActiveUsers = data.Level9Users != null ? data.Level9Users : 0,
                    Level9Amount = data.Level9 != null ? data.Level1Users : 0,

                    Level10ActiveUsers = data.Level10Users != null ? data.Level10Users : 0,
                    Level10Amount = data.Level10 != null ? data.Level10Users : 0,
                };
            }
            else
            {
                return new LevelEarningModelMaster()
                {
                    Level1ActiveUsers =  0,
                    Level1Amount =  0,

                    Level2ActiveUsers =  0,
                    Level2Amount =  0,

                    Level3ActiveUsers =  0,
                    Level3Amount = 0,

                    Level4ActiveUsers =  0,
                    Level4Amount =  0,

                    Level5ActiveUsers =  0,
                    Level5Amount =  0,

                    Level6ActiveUsers =  0,
                    Level6Amount =  0,

                    Level7ActiveUsers =  0,
                    Level7Amount =  0,

                    Level8ActiveUsers =  0,
                    Level8Amount =  0,

                    Level9ActiveUsers =  0,
                    Level9Amount =  0,

                    Level10ActiveUsers = 0,
                    Level10Amount =  0,
                };
            }
        }
        #endregion

        #region Add Level Base Earning Amount
        public string ADDLevelBaseEarningAmount()
        {
            var activeUsers = entities.Users.Where(x => x.isActive == true).ToList();
            if (activeUsers.Any())
            {
                EaningHeadModel earningHeads = new EaningHeadModel();
                var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
                using (StreamReader r = new StreamReader(jsonFilePath))
                {
                    string json = r.ReadToEnd();
                     earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
                }
                foreach (var item in activeUsers)
                { 
                    var childUsers = activeUsers.Where(x => x.ParentIDs.Contains(item.UserID) && x.LastActiveDate == DateTime.Now.AddDays(-1)).ToList();

                    if(childUsers.Any())
                    {
                        foreach (var level in childUsers)
                        {
                            var parentIDs = level.ParentIDs.Split(',').ToList();
                            var parentUserWithLevel = parentIDs.Select(x => new SetLevelForParentUser()
                            {
                                UserId = x,
                                Level = parentIDs.IndexOf(x) + 1,
                                Count = parentIDs.Count()
                            }).ToList();

                            var firtLevel= parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 1).FirstOrDefault();


                            //string[] s = level.ParentIDs.Split(',');
                            //if(s.Length==1)
                            //{
                            //    int data = 1;
                            //}
                        }
                    }

                    //if (item.ParentIDs != null)
                    //{
                    //    var parentIDs = item.ParentIDs.Split(',').ToList();
                    //    var parentUserWithLevel = parentIDs.Select(s => new SetLevelForParentUser()
                    //    {
                    //        UserId = s,
                    //        Level = parentIDs.IndexOf(s) + 1
                    //    }).ToList();

                    //    foreach(var pu in parentUserWithLevel)
                    //    {

                    //        if(pu.Level == 1)
                    //        {
                    //            var earning = earningHeads.Level1Income;
                    //        }
                    //        else if (pu.Level == 2)
                    //        {
                    //            var earning = earningHeads.Level1Income;
                    //        }
                    //    }

                    //    //var parentUserLevelEarning = (from ac in activeUsers
                    //    //                              join ul in userWithLevel on ac.UserID equals ul.UserId
                    //    //                              select new { ac, ul
                    //    //                              }).ToList();

                        
                    //}
                }
            }
            else
            {
                return null;
            }
                return string.Empty;
        }
        #endregion
    }
}