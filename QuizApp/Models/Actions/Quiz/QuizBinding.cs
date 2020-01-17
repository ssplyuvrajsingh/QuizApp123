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
        public List<QuizResult> GetQuiz(string userId)
        {
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();
            //var data = (from a in entities.QuizDatas
            //            join qp in entities.QuizPlayers on a.QuizID equals qp.QuizID
            //            where   (qp.IsCompleted != true &&  qp.QuizID == a.QuizID && qp.UserID == userId) || a.isActive ==true
            //            select new QuizResult()
            //            {
            //                QuizID = a.QuizID,
            //                QuizTitle = a.QuizTitle,
            //                QuizBannerImage = ImageSource + a.QuizBannerImage,
            //                NoOfQuestion = a.NoOfQuestion.Value,
            //                PlayingDescriptionImg = ImageSource + a.PlayingDescriptionImg,
            //                StartDate = a.StartDate.Value,
            //                WinPrecentage = a.WinPrecentage
            //            }).ToList();

            // Filter data that match with today date
            var quizDatas = entities.QuizDatas.ToList();
            var data = quizDatas.Where(x => x.isActive == true).OrderBy(x=>x.CreatedDate).Select(a => new QuizResult()
            {
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,
                
                QuizBannerImage =  a.QuizBannerImage!=null? ImageSource + a.QuizBannerImage:ImageSource + "/Content/attachment/bd574fd3-1faa-4884-a152-fd55e6dffe3a_Untitled-1.png",
                NoOfQuestion = a.NoOfQuestion.Value,
                
                PlayingDescriptionImg =  a.PlayingDescriptionImg != null ? ImageSource + a.PlayingDescriptionImg : ImageSource + "/Content/attachment/bd574fd3-1faa-4884-a152-fd55e6dffe3a_Untitled-1.png",

                StartDate = a.StartDate.Value,
                WinPrecentage = a.WinPrecentage
            }).ToList();

            List<QuizResult> quizResults = new List<QuizResult>();

            foreach (var item in data)
            {
                var f = new QuizResult();
                var data1 = entities.QuizPlayers.Where(x => x.UserID == userId && x.QuizID == item.QuizID && x.IsCompleted == true).FirstOrDefault();
                if (data1 == null)
                {
                    quizResults.Add(item);
                }
            }
            if (quizResults.Count == 0)
            {
                User user = new User()
                {
                    LastActiveDate = DateTime.Now
                };
            }
            data.ForEach(a =>
            {
                a.StartDateStr = a.StartDate.ToString("dd MMM hh:mm tt");
                a.isActive = DateTime.Compare(a.StartDate, DateTime.Now) <= 0;
            });

            return quizResults;
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
                var QuizQuestions = entities.QuizQuestions.Where(x => x.QuizID == quizId).OrderBy(X => X.CreatedDate).ToList();/*OrderBy(r => Guid.NewGuid()).Take(quiz.NoOfQuestion.Value).ToList();*/
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
        public UserWalletModel GetWalletInfo(UserModel model)
        {
            UserWalletModel userWallet = new UserWalletModel();
            var data = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
            List<TransactionModel> transactionModels = new List<TransactionModel>();
            var transactions = entities.Transactions.Where(x => x.UserID == model.UserId).ToList();
            foreach (var a in transactions)
            {
                    var Tran = new TransactionModel();
                    Tran.transactionDateTime = string.Format("{0: dd MMMM }", a.transactionDateTime) + " " + string.Format("{0:hh:mm tt}", a.transactionDateTime);
                    Tran.amount = Math.Truncate((double)a.amount);
                    Tran.paymentStatus = a.paymentStatus;
                    transactionModels.Add(Tran);  
            }
            userWallet.TransactionModels = transactionModels;
            userWallet.CurrentBalance = Math.Truncate(data.CurrentBalance != null ? (double)data.CurrentBalance : 0);
            userWallet.MothlyIncome = Math.Truncate(data.MothlyIncome != null ? (double)data.MothlyIncome : 0);
            userWallet.TotalWithdraw = Math.Truncate(data.TotalWithdraw != null ? (double)data.TotalWithdraw : 0);
            userWallet.TotalPoins = data.CurrentPoint!=null?(int)data.CurrentPoint:0;
            return userWallet;
        }
        #endregion

        #region Get Level Base Earning Amount
        public LevelEarningModelMaster GetLevelBaseEarningAmount(string Users)
        {
            LevelEarningModelMaster levelEarningModelMaster = new LevelEarningModelMaster();
            var data = entities.LevelEarnings.Where(x => x.UserID == Users).FirstOrDefault();
            List<LevelEarningModel> levelEarnings = new List<LevelEarningModel>();
            for (int i = 1; i <= 10; i++)
            {
                var lvl = new LevelEarningModel();
                switch (i)
                {
                    case 1:
                        lvl.Title = "Level 1";
                        lvl.Activeuser = data != null ? data.Level1Users != null ? (int)data.Level1Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level1 != null ? (double)data.Level1 : 0 : 0;
                        break;

                    case 2:
                        lvl.Title = "Level 2";
                        lvl.Activeuser = data != null ? data.Level2Users != null ? (int)data.Level2Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level2 != null ? (double)data.Level2 : 0 : 0;
                        break;
                    case 3:
                        lvl.Title = "Level 3";
                        lvl.Activeuser = data != null ? data.Level3Users != null ? (int)data.Level3Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level3 != null ? (double)data.Level3 : 0 : 0;
                        break;
                    case 4:
                        lvl.Title = "Level 4";
                        lvl.Activeuser = data != null ? data.Level4Users != null ? (int)data.Level4Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level4 != null ? (double)data.Level4 : 0 : 0;
                        break;
                    case 5:
                        lvl.Title = "Level 5";
                        lvl.Activeuser = data != null ? data.Level5Users != null ? (int)data.Level5Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level5 != null ? (double)data.Level5 : 0 : 0;
                        break;
                    case 6:
                        lvl.Title = "Level 6";
                        lvl.Activeuser = data != null ? data.Level6Users != null ? (int)data.Level6Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level6 != null ? (double)data.Level6 : 0 : 0;
                        break;
                    case 7:
                        lvl.Title = "Level 7";
                        lvl.Activeuser = data != null ? data.Level7Users != null ? (int)data.Level7Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level7 != null ? (double)data.Level7 : 0 : 0;
                        break;
                    case 8:
                        lvl.Title = "Level 8";
                        lvl.Activeuser = data != null ? data.Level8Users != null ? (int)data.Level8Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level8 != null ? (double)data.Level8 : 0 : 0;
                        break;
                    case 9:
                        lvl.Title = "Level 9";
                        lvl.Activeuser = data != null ? data.Level9Users != null ? (int)data.Level9Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level9 != null ? (double)data.Level9 : 0 : 0;
                        break;
                    case 10:
                        lvl.Title = "Level 10";
                        lvl.Activeuser = data != null ? data.Level10Users != null ? (int)data.Level10Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level10 != null ? (double)data.Level10 : 0 : 0;
                        break;
                }
                levelEarnings.Add(lvl);
            }
            levelEarningModelMaster.levelEarnings = levelEarnings;
            return levelEarningModelMaster;
        }
        #endregion

        #region Add Level Base Earning Amount
        public bool AddLevelBaseEarningAmount(string jsonFilePath)
        {
            try
            {
                var activeUsers = entities.Users.Where(x => x.isActive == true).ToList();
                activeUsers = activeUsers.Where(x => x.LastActiveDate != null && (x.LastActiveDate.Value).Date == (DateTime.Now.AddDays(-1)).Date).ToList();
                if (activeUsers.Any())
                {
                    EaningHeadModel earningHeads = new EaningHeadModel();
                    using (StreamReader r = new StreamReader(jsonFilePath))
                    {
                        string json = r.ReadToEnd();
                        earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
                    }
                    foreach (var item in activeUsers)
                    {
                        double totalTransactionAmt = 0;
                        List<LevelWithUser> lstLevelWithUser = new List<LevelWithUser>();
                        var childUsers = activeUsers.Where(x => !string.IsNullOrEmpty(x.ParentIDs) && x.ParentIDs.Split(',').Where(y => y == item.UserID).Any()).ToList();
                        totalTransactionAmt += earningHeads.DirectIncome;
                        if (childUsers.Any())
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

                                var level_1 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 1).FirstOrDefault();
                                if (level_1 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 1).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l1 = new LevelWithUser();
                                        l1.Level = 1;
                                        l1.ChildUsers = new List<ChildUser>();
                                        l1.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l1);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_2 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 2).FirstOrDefault();
                                if (level_2 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 2).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l2 = new LevelWithUser();
                                        l2.Level = 2;
                                        l2.ChildUsers = new List<ChildUser>();
                                        l2.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l2);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }

                                }

                                var level_3 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 3).FirstOrDefault();
                                if (level_3 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 3).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l3 = new LevelWithUser();
                                        l3.Level = 3;
                                        l3.ChildUsers = new List<ChildUser>();
                                        l3.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l3);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_4 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 4).FirstOrDefault();
                                if (level_4 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 4).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l4 = new LevelWithUser();
                                        l4.Level = 4;
                                        l4.ChildUsers = new List<ChildUser>();
                                        l4.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l4);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_5 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 5).FirstOrDefault();
                                if (level_5 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 5).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l5 = new LevelWithUser();
                                        l5.Level = 5;
                                        l5.ChildUsers = new List<ChildUser>();
                                        l5.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l5);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_6 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 6).FirstOrDefault();
                                if (level_6 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 6).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l6 = new LevelWithUser();
                                        l6.Level = 6;
                                        l6.ChildUsers = new List<ChildUser>();
                                        l6.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l6);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_7 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 7).FirstOrDefault();
                                if (level_7 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 7).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l7 = new LevelWithUser();
                                        l7.Level = 7;
                                        l7.ChildUsers = new List<ChildUser>();
                                        l7.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l7);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_8 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 8).FirstOrDefault();
                                if (level_8 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 8).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l8 = new LevelWithUser();
                                        l8.Level = 8;
                                        l8.ChildUsers = new List<ChildUser>();
                                        l8.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l8);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_9 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 9).FirstOrDefault();
                                if (level_9 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 9).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l9 = new LevelWithUser();
                                        l9.Level = 9;
                                        l9.ChildUsers = new List<ChildUser>();
                                        l9.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l9);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }

                                var level_10 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 10).FirstOrDefault();
                                if (level_10 != null)
                                {
                                    var data = lstLevelWithUser.Where(x => x.Level == 10).FirstOrDefault();
                                    if (data == null)
                                    {
                                        var l10 = new LevelWithUser();
                                        l10.Level = 10;
                                        l10.ChildUsers = new List<ChildUser>();
                                        l10.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                        lstLevelWithUser.Add(l10);
                                    }
                                    else
                                    {
                                        data.ChildUsers.Add(new ChildUser()
                                        {
                                            UserId = level.UserID
                                        });
                                    }
                                }
                            }

                            if (lstLevelWithUser.Any())
                            {
                                var le = new LevelEarning();
                                var userEarningExist = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
                                foreach (var lst in lstLevelWithUser)
                                {
                                    int userCount = 0; double actualEarning = 0;
                                    if (userEarningExist != null)
                                    {
                                        if (lst.Level == 1)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                            userEarningExist.Level1 = actualEarning;
                                            userEarningExist.Level1Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 2)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level2Income / 30)), 2);
                                            userEarningExist.Level2 = actualEarning;
                                            userEarningExist.Level2Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 3)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level3Income / 30)), 2);
                                            userEarningExist.Level3 = actualEarning;
                                            userEarningExist.Level3Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 4)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level4Income / 30)), 2);
                                            userEarningExist.Level4 = actualEarning;
                                            userEarningExist.Level4Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 5)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level5Income / 30)), 2);
                                            userEarningExist.Level5 = actualEarning;
                                            userEarningExist.Level5Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 6)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level6Income / 30)), 2);
                                            userEarningExist.Level6 = actualEarning;
                                            userEarningExist.Level6Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 7)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level7Income / 30)), 2);
                                            userEarningExist.Level7 = actualEarning;
                                            userEarningExist.Level7Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 8)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level8Income / 30)), 2);
                                            userEarningExist.Level8 = actualEarning;
                                            userEarningExist.Level8Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 9)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level9Income / 30)), 2);
                                            userEarningExist.Level9 = actualEarning;
                                            userEarningExist.Level9Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 10)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level10Income / 30)), 2);
                                            userEarningExist.Level10 = actualEarning;
                                            userEarningExist.Level10Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                    }
                                    else
                                    {
                                        le.UserID = item.UserID;
                                        if (lst.Level == 1)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                            le.Level1 = actualEarning;
                                            le.Level1Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 2)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level2Income / 30)), 2);
                                            le.Level2 = actualEarning;
                                            le.Level2Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 3)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level3Income / 30)), 2);
                                            le.Level3 = actualEarning;
                                            le.Level3Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 4)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level4Income / 30)), 2);
                                            le.Level4 = actualEarning;
                                            le.Level4Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 5)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level5Income / 30)), 2);
                                            le.Level5 = actualEarning;
                                            le.Level5Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 6)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level6Income / 30)), 2);
                                            le.Level6 = actualEarning;
                                            le.Level6Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 7)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level7Income / 30)), 2);
                                            le.Level7 = actualEarning;
                                            le.Level7Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 8)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level8Income / 30)), 2);
                                            le.Level8 = actualEarning;
                                            le.Level8Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 9)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level9Income / 30)), 2);
                                            le.Level9 = actualEarning;
                                            le.Level9Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                        if (lst.Level == 10)
                                        {
                                            userCount = lst.ChildUsers.Count();
                                            actualEarning = Math.Round((userCount * (earningHeads.Level10Income / 30)), 2);
                                            le.Level10 = actualEarning;
                                            le.Level10Users = userCount;
                                            totalTransactionAmt += actualEarning;
                                        }
                                    }
                                }

                                if (userEarningExist == null)
                                {
                                    entities.LevelEarnings.Add(le);
                                }
                                entities.SaveChanges();
                            }
                        }

                        //---------------- Make Transaction for Each user
                        var ud = entities.Users.FirstOrDefault(f => f.UserID == item.UserID);
                        var uniqueKey = $"{item.UserID}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                        var transaction = new Transaction()
                        {
                            UserID = item.UserID,
                            transactionDateTime = DateTime.Now,
                            UniqueKey = uniqueKey,
                            paymentStatus = "Earning",
                            amount = totalTransactionAmt,
                            username = ud?.Name,
                            mobilenumber = ud?.AspNetUser.UserName
                        };

                        entities.Transactions.Add(transaction);
                        entities.SaveChanges();
                    }

                    return true;
                }
            }
            catch (Exception ex1)
            {
                MailSenderRepo mailSenderRepo = new MailSenderRepo();
                mailSenderRepo.MailSender("rajkumar@sumedhasoftech.com", ex1.Message + ex1.StackTrace, "Quiz Exception");
            }

            return false;
        }
        #endregion
    }
}