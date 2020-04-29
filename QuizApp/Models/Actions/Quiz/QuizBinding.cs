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
        public UserQuizWallet GetQuiz(string userId)
        {
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();
            var kk = entities.QuizDatas.Where(x => (x.IsDeleted == null || x.IsDeleted.Value == false) && x.isActive == true).OrderByDescending(x => x.CreatedDate).ToList();
            var data = entities.QuizDatas.Where(x => (x.IsDeleted == null || x.IsDeleted.Value == false) && x.isActive == true).OrderByDescending(x => x.CreatedDate).Select(a => new QuizResult()
            {
                QuizID = a.QuizID,
                QuizTitle = a.QuizTitle,

                QuizBannerImage = a.QuizBannerImage != null ? ImageSource + a.QuizBannerImage : ImageSource + "/Content/attachment/bd574fd3-1faa-4884-a152-fd55e6dffe3a_Untitled-1.png",
                NoOfQuestion = a.NoOfQuestion.Value,

                PlayingDescriptionImg = a.PlayingDescriptionImg != null ? ImageSource + a.PlayingDescriptionImg : ImageSource + "/Content/attachment/bd574fd3-1faa-4884-a152-fd55e6dffe3a_Untitled-1.png",
                AlreadyPlayed = false,
                StartDate = a.StartDate.Value,
                WinPrecentage = a.WinPrecentage
            }).ToList();
            //Check Player Already played this Quiz
            foreach (var item in data)
            {
                var existingData = entities.QuizPlayers.Where(a => a.QuizID == item.QuizID && a.UserID == userId && a.IsCompleted == true).OrderByDescending(a => a.PlayedDate).FirstOrDefault();
                if (existingData != null)
                {
                    item.AlreadyPlayed = true;
                    item.WinPrecentage = (int)existingData.PercentageEarn;
                }

            }
            //Convert to a specific Formate DateTime 
            data.ForEach(a =>
            {
                a.StartDateStr = a.StartDate.ToString("dd MMM yyyy");
                a.isActive = DateTime.Compare(a.StartDate, DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)) <= 0;
            });
            //Update Last Active Date User
            var playedDataCount = entities.QuizPlayers.Where(a => a.UserID == userId && a.IsCompleted == true && a.PlayedDate != null).OrderByDescending(a => a.PlayedDate).ToList();
            //var d = DateTime.Now.Date;
            playedDataCount = playedDataCount.Where(a => (a.PlayedDate.Value).Date == (DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)).Date).OrderByDescending(a => a.PlayedDate).ToList();
            var eaningHeadModel = GetEaningHead();
            if (playedDataCount.Count() >= eaningHeadModel.MinimumQuiz)
            {
                var LastActiveDate = entities.Users.Where(x => x.UserID == userId).FirstOrDefault();
                LastActiveDate.LastActiveDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                entities.SaveChanges();
            }

            var userWallet = GetWalletInfo(new UserModel() { UserId = userId });
            var earningHeads = GetEarningHeadModel();
            var model = new UserQuizWallet()
            {
                CurrentBalance = userWallet.CurrentBalance,
                MothlyIncome = userWallet.MothlyIncome,
                TotalWithdraw = userWallet.TotalWithdraw,
                MinimumQuiz = earningHeads != null ? earningHeads.MinimumQuiz : 0,
                QuizList = data
            };

            return model;
        }
        #endregion

        #region Quiz Questions
        public QuizQuestionResultMain GetQuizQuestions(Guid quizId, string UserId)
        {
            var ImageSource = ConfigurationManager.AppSettings["ImageSource"].ToString();

            QuizQuestionResultMain quizQuestionResultMain = new QuizQuestionResultMain();
            var quiz = entities.QuizDatas.Where(a => (a.IsDeleted == null || a.IsDeleted.Value == false) && a.QuizID == quizId).FirstOrDefault();
            if (quiz != null)
            {
                List<QuizQuestionResult> lstQuizQuestionResult = new List<QuizQuestionResult>();
                var QuizQuestions = entities.QuizQuestions.Where(x => (x.IsDeleted == null || x.IsDeleted.Value == false) && x.QuizID == quizId).OrderBy(X => X.CreatedDate).ToList();
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
                if (existingData != null)
                {
                    quizQuestionResultMain.AlreadyPlayed = true;
                    quizQuestionResultMain.IsCompleted = existingData.IsCompleted.Value;
                    quizQuestionResultMain.IsWon = existingData.IsWon.Value;
                    quizQuestionResultMain.PlayedDate = existingData.PlayedDate.Value.ToString("dd mmm, yyyy");
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
            try
            {
                var player = new QuizPlayer()
                {
                    CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                    IsCompleted = false,
                    IsWon = false,
                    Language = "",
                    PercentageEarn = 0,
                    PlayedDate = DateTime.UtcNow,
                    PointEarn = 0,
                    QuizID = model.QuizId,
                    UserID = model.UserId
                };
                entities.QuizPlayers.Add(player);
                entities.SaveChanges();
                return player.PlayerID;
            }
            catch(Exception ex)
            {
                var data = ex.Message;
                return 0;
            }
        }
        #endregion

        #region Set Quize Player
        public EndGameResult SetQuizePlayer(QuizPlayerResult model)
        {
            #region Set User Answer
            foreach (var item in model.UserAnswer)
            {
                var quizQuestion = entities.QuizQuestions.Where(a => (a.IsDeleted == null || a.IsDeleted.Value == false) && a.QuizQuestionID == item.QuizQuestionID).FirstOrDefault();
                item.PointEarn = 0;
                if (quizQuestion != null && quizQuestion.CorrectOption == item.SelectedOption)
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
                    CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
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
            var quizData = entities.QuizDatas.Where(x => (x.IsDeleted == null || x.IsDeleted.Value == false) && x.QuizID == model.QuizID).FirstOrDefault();
            if (quizData != null)
            {
                int percentageEarn = 0;
                int userAnswerTotal = (int)entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.PointEarn);
                //Get percentage earn
                var quizQuest = entities.QuizQuestions.Where(x => (x.IsDeleted == null || x.IsDeleted.Value == false) && x.QuizID == model.QuizID).ToList();
                if (quizQuest.Any())
                {
                    percentageEarn = (userAnswerTotal * 100) / (int)quizQuest.Sum(x => x.QuestionPoint);
                }
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
                    TransactionDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                    PointsEarned = userAnswerTotal,
                    PointsWithdraw = 0,
                    Description = "Point earnd from quiz id : " + model.QuizID,
                    CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
                };
                entities.UserPoints.Add(userPoint);

                

                //Update QuizPlayer
                var quizPlayer = entities.QuizPlayers.Where(x => x.PlayerID == model.PlayerID).FirstOrDefault();
                int Second = entities.UserAnswers.Where(x => x.PlayerID == model.PlayerID).Sum(x => x.TimeTaken);
                TimeSpan time = TimeSpan.FromSeconds(Second);
                if (quizPlayer != null)
                {
                    QuizPlayer player = new QuizPlayer()
                    {
                        PlayerID = quizPlayer.PlayerID,
                        QuizID = model.QuizID,
                        IsCompleted = true,
                        IsWon = isWon,
                        PointEarn = userAnswerTotal,
                        PlayedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                        PercentageEarn = percentageEarn,
                        Language = "English",
                        UserID = quizPlayer.UserID,
                        CreatedDate = quizPlayer.CreatedDate,
                        TotalTimeTaken = time
                    };

                    entities.Entry(quizPlayer).CurrentValues.SetValues(player);
                }
                entities.SaveChanges();

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
            int.TryParse(entities.QuizDatas.Where(x => (x.IsDeleted == null || x.IsDeleted.Value==false) && x.QuizID == model.QuizID).FirstOrDefault()?.MaxPoint.ToString(), out int OverallScore);
            if (quizPlayer != null)
            {
                int playerBestScore = (int)quizPlayer.Max(x => x.PointEarn);
                return new GetScoreResult()
                {
                    YourBestScore = playerBestScore,
                    OverallScore = OverallScore
                };
            }
            else
            {
                return new GetScoreResult()
                {
                    YourBestScore = 0,
                    OverallScore = OverallScore
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
            var transactions = entities.Transactions.Where(x => x.UserID == model.UserId).OrderByDescending(x => x.transactionDateTime).ToList();
            foreach (var a in transactions)
            {
                var Tran = new TransactionModel();
                Tran.transactionDateTime = string.Format("{0: dd MMMM }", a.transactionDateTime) + " " + string.Format("{0:hh:mm tt}", a.transactionDateTime);
                Tran.paymentStatus = a.paymentStatus;
                transactionModels.Add(Tran);
                double x;
                Double.TryParse(a.amount.ToString(), out x);
                Tran.amount = x.ToString("0.00");
            }
            GeneralFunctions generalFunctions = new GeneralFunctions();
            userWallet.TransactionModels = transactionModels;
            userWallet.CurrentBalance = data.CurrentBalance != null ? Convert.ToDouble(generalFunctions.GetDecimalvalue(data.CurrentBalance.ToString())) : 0;
            userWallet.MothlyIncome = data.MothlyIncome != null ? Convert.ToDouble(generalFunctions.GetDecimalvalue(data.MothlyIncome.ToString())) : 0;
            userWallet.TotalWithdraw = data.TotalWithdraw != null ? Convert.ToDouble(generalFunctions.GetDecimalvalue(data.TotalWithdraw.ToString())) : 0;
            userWallet.TotalPoins = data.CurrentPoint != null ? (int)data.CurrentPoint : 0;
            return userWallet;
        }
        #endregion

        #region Get Level Base Earning Amount
        public LevelEarningModelMaster GetLevelBaseEarningAmount(string Users)
        {
            //Update Active Users and Monthly Income
            GeneralFunctions general = new GeneralFunctions();
            var earningHeads = general.getEarningHeads();

            RefreshLevelBaseActiveUsersandMonthlyIncome(Users);
            LevelEarningModelMaster levelEarningModelMaster = new LevelEarningModelMaster();
            var data = entities.LevelEarnings.Where(x => x.UserID == Users).FirstOrDefault();
            List<LevelEarningModel> levelEarnings = new List<LevelEarningModel>();
            for (int i = 1; i <= 10; i++)
            {
                var lvl = new LevelEarningModel();
                switch (i)
                {
                    case 1:
                        lvl.Level = 1;
                        lvl.Title = "Level 1";
                        lvl.Activeuser = data != null ? data.Level1Users != null ? (int)data.Level1Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level1 != null ? (double)data.Level1 : 0 : 0;
                        break;

                    case 2:
                        lvl.Level = 2;
                        lvl.Title = "Level 2";
                        lvl.Activeuser = data != null ? data.Level2Users != null ? (int)data.Level2Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level2 != null ? (double)data.Level2 : 0 : 0;
                        break;
                    case 3:
                        lvl.Level = 3;
                        lvl.Title = "Level 3";
                        lvl.Activeuser = data != null ? data.Level3Users != null ? (int)data.Level3Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level3 != null ? (double)data.Level3 : 0 : 0;
                        break;
                    case 4:
                        lvl.Level = 4;
                        lvl.Title = "Level 4";
                        lvl.Activeuser = data != null ? data.Level4Users != null ? (int)data.Level4Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level4 != null ? (double)data.Level4 : 0 : 0;
                        break;
                    case 5:
                        lvl.Level = 5;
                        lvl.Title = "Level 5";
                        lvl.Activeuser = data != null ? data.Level5Users != null ? (int)data.Level5Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level5 != null ? (double)data.Level5 : 0 : 0;
                        break;
                    case 6:
                        lvl.Level = 6;
                        lvl.Title = "Level 6";
                        lvl.Activeuser = data != null ? data.Level6Users != null ? (int)data.Level6Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level6 != null ? (double)data.Level6 : 0 : 0;
                        break;
                    case 7:
                        lvl.Level = 7;
                        lvl.Title = "Level 7";
                        lvl.Activeuser = data != null ? data.Level7Users != null ? (int)data.Level7Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level7 != null ? (double)data.Level7 : 0 : 0;
                        break;
                    case 8:
                        lvl.Level = 8;
                        lvl.Title = "Level 8";
                        lvl.Activeuser = data != null ? data.Level8Users != null ? (int)data.Level8Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level8 != null ? (double)data.Level8 : 0 : 0;
                        break;
                    case 9:
                        lvl.Level = 9;
                        lvl.Title = "Level 9";
                        lvl.Activeuser = data != null ? data.Level9Users != null ? (int)data.Level9Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level9 != null ? (double)data.Level9 : 0 : 0;
                        break;
                    case 10:
                        lvl.Level = 10;
                        lvl.Title = "Level 10";
                        lvl.Activeuser = data != null ? data.Level10Users != null ? (int)data.Level10Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level10 != null ? (double)data.Level10 : 0 : 0;
                        break;
                }
                levelEarnings.Add(lvl);
            }
            levelEarningModelMaster.levelEarnings = levelEarnings;
            levelEarningModelMaster.MinimumQuiz = earningHeads!=null ? earningHeads.MinimumQuiz : 0;
            return levelEarningModelMaster;
        }
        #endregion

        #region Add Level Base Earning Amount
        public bool CallLevelBaseEarningAmount()
        {
            var data = AddLevelBaseEarningAmount();
            LevelFirebaseUpdates(data);
            return data;
        }
        public bool AddLevelBaseEarningAmount()
        {
            try
            {
               var activeUsers = entities.Users.Where(x => x.isActive == true).ToList();
                //activeUsers = activeUsers.Where(x => x.LastActiveDate != null).ToList();
                if (activeUsers.Any())
                {
                    GeneralFunctions general = new GeneralFunctions();
                    var earningHeads = general.getEarningHeads();
                    //EaningHeadModel earningHeads = new EaningHeadModel();

                    //using (StreamReader r = new StreamReader(jsonFilePath))
                    //{
                    //    string json = r.ReadToEnd();
                    //    earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
                    //}
                    foreach (var item in activeUsers)
                    {
                        //Check Last Active Date
                        if (item.LastActiveDate != null)
                        {
                            if ((item.LastActiveDate.Value).Date == DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00).AddDays(-1).Date)
                            {
                                double totalTransactionAmt = 0;
                                List<LevelWithUser> lstLevelWithUser = new List<LevelWithUser>();
                                //Get Child Users
                                var childUsers = activeUsers.Where(x => !string.IsNullOrEmpty(x.ParentIDs) && x.ParentIDs.Split(',').Where(y => y == item.UserID).Any()).ToList();
                                totalTransactionAmt += earningHeads.DirectIncome / 30;
                                if (childUsers.Any())
                                {
                                    foreach (var level in childUsers)
                                    {
                                        if (level.LastActiveDate != null)
                                        {
                                            if ((level.LastActiveDate.Value).Date == DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00).AddDays(-1).Date)
                                            {
                                                var parentIDs = level.ParentIDs.Split(',').ToList();
                                                var parentUserWithLevel = parentIDs.Select(x => new SetLevelForParentUser()
                                                {
                                                    UserId = x,
                                                    Level = parentIDs.IndexOf(x) + 1,
                                                    Count = parentIDs.Count()
                                                }).ToList();

                                                //Add Level 
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
                                        }
                                    }

                                    //Add Actual Earning Based on Level
                                    if (lstLevelWithUser.Any())
                                    {
                                        var le = new LevelEarning();
                                        GeneralFunctions generalFunctions = new GeneralFunctions();
                                        var userEarningExist = new LevelUsersModel();
                                        var userOld = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
                                        foreach (var lst in lstLevelWithUser)
                                        {
                                            int userCount = 0; double actualEarning = 0;

                                            //Already this User Exist check Condition then Update
                                            if (userOld != null)
                                            {
                                                if (lst.Level == 1)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                                    userEarningExist.Level1 = actualEarning;
                                                    userEarningExist.Level1Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }

                                                if (lst.Level == 2)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level2Income / 30)), 2);
                                                    userEarningExist.Level2 = actualEarning;
                                                    userEarningExist.Level2Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 3)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level3Income / 30)), 2);
                                                    userEarningExist.Level3 = actualEarning;
                                                    userEarningExist.Level3Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 4)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level4Income / 30)), 2);
                                                    userEarningExist.Level4 = actualEarning;
                                                    userEarningExist.Level4Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 5)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level5Income / 30)), 2);
                                                    userEarningExist.Level5 = actualEarning;
                                                    userEarningExist.Level5Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 6)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level6Income / 30)), 2);
                                                    userEarningExist.Level6 = actualEarning;
                                                    userEarningExist.Level6Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 7)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level7Income / 30)), 2);
                                                    userEarningExist.Level7 = actualEarning;
                                                    userEarningExist.Level7Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 8)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level8Income / 30)), 2);
                                                    userEarningExist.Level8 = actualEarning;
                                                    userEarningExist.Level8Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 9)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level9Income / 30)), 2);
                                                    userEarningExist.Level9 = actualEarning;
                                                    userEarningExist.Level9Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 10)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level10Income / 30)), 2);
                                                    userEarningExist.Level10 = actualEarning;
                                                    userEarningExist.Level10Users = userCount;
                                                    userEarningExist.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                            }
                                            else
                                            {
                                                //If Not Exist then Insert 
                                                le.UserID = item.UserID;
                                                if (lst.Level == 1)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                                    le.Level1 = actualEarning;
                                                    le.Level1Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 2)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level2Income / 30)), 2);
                                                    le.Level2 = actualEarning;
                                                    le.Level2Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 3)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level3Income / 30)), 2);
                                                    le.Level3 = actualEarning;
                                                    le.Level3Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 4)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level4Income / 30)), 2);
                                                    le.Level4 = actualEarning;
                                                    le.Level4Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 5)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level5Income / 30)), 2);
                                                    le.Level5 = actualEarning;
                                                    le.Level5Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 6)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level6Income / 30)), 2);
                                                    le.Level6 = actualEarning;
                                                    le.Level6Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 7)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level7Income / 30)), 2);
                                                    le.Level7 = actualEarning;
                                                    le.Level7Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 8)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level8Income / 30)), 2);
                                                    le.Level8 = actualEarning;
                                                    le.Level8Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 9)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level9Income / 30)), 2);
                                                    le.Level9 = actualEarning;
                                                    le.Level9Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                                if (lst.Level == 10)
                                                {
                                                    userCount = lst.ChildUsers.Count();
                                                    actualEarning = Math.Round((userCount * (earningHeads.Level10Income / 30)), 2);
                                                    le.Level10 = actualEarning;
                                                    le.Level10Users = userCount;
                                                    le.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                                    totalTransactionAmt += actualEarning;
                                                }
                                            }
                                        }

                                        if (userOld == null)
                                        {
                                            entities.LevelEarnings.Add(le);
                                            entities.SaveChanges();
                                        }
                                        else
                                        {
                                            if (userEarningExist.Level1 == null)
                                            {
                                                userEarningExist.Level1 = 0;
                                                userEarningExist.Level1Users = 0;
                                            }
                                            if (userEarningExist.Level2 == null)
                                            {
                                                userEarningExist.Level2 = 0;
                                                userEarningExist.Level2Users = 0;
                                            }
                                            if (userEarningExist.Level3 == null)
                                            {
                                                userEarningExist.Level3 = 0;
                                                userEarningExist.Level3Users = 0;
                                            }
                                            if (userEarningExist.Level4 == null)
                                            {
                                                userEarningExist.Level4 = 0;
                                                userEarningExist.Level4Users = 0;
                                            }
                                            if (userEarningExist.Level5 == null)
                                            {
                                                userEarningExist.Level5 = 0;
                                                userEarningExist.Level5Users = 0;
                                            }
                                            if (userEarningExist.Level6 == null)
                                            {
                                                userEarningExist.Level6 = 0;
                                                userEarningExist.Level6Users = 0;
                                            }
                                            if (userEarningExist.Level7 == null)
                                            {
                                                userEarningExist.Level7 = 0;
                                                userEarningExist.Level7Users = 0;
                                            }
                                            if (userEarningExist.Level8 == null)
                                            {
                                                userEarningExist.Level8 = 0;
                                                userEarningExist.Level8Users = 0;
                                            }
                                            if (userEarningExist.Level9 == null)
                                            {
                                                userEarningExist.Level9 = 0;
                                                userEarningExist.Level9Users = 0;
                                            }
                                            if (userEarningExist.Level10 == null)
                                            {
                                                userEarningExist.Level10 = 0;
                                                userEarningExist.Level10Users = 0;
                                            }
                                            //db.Entry(old).CurrentValues.SetValues(model);
                                            entities.Entry(userOld).CurrentValues.SetValues(userEarningExist);
                                            entities.SaveChanges();
                                        }

                                    }
                                }
                                //when Child Users not Exists of activeUsers 
                                else
                                {
                                    var CNE = new LevelEarning();
                                    var childUsersNotExists = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
                                    //Entery Update When Active Users Already Existes in Level Earning Table
                                    if (childUsersNotExists == null)
                                    {
                                        CNE.UserID = item.UserID;
                                        CNE.Level1 = 0;
                                        CNE.Level2 = 0;
                                        CNE.Level3 = 0;
                                        CNE.Level4 = 0;
                                        CNE.Level5 = 0;
                                        CNE.Level6 = 0;
                                        CNE.Level7 = 0;
                                        CNE.Level8 = 0;
                                        CNE.Level9 = 0;
                                        CNE.Level10 = 0;
                                        CNE.Level1Users = 0;
                                        CNE.Level2Users = 0;
                                        CNE.Level3Users = 0;
                                        CNE.Level4Users = 0;
                                        CNE.Level5Users = 0;
                                        CNE.Level6Users = 0;
                                        CNE.Level7Users = 0;
                                        CNE.Level8Users = 0;
                                        CNE.Level9Users = 0;
                                        CNE.Level10Users = 0;
                                        CNE.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                        entities.LevelEarnings.Add(CNE);
                                        entities.SaveChanges();
                                    }
                                    //Entery Insert When Active Users Already not Existes in Level Earning Table
                                    else
                                    {
                                        childUsersNotExists.Level1 = 0;
                                        childUsersNotExists.Level2 = 0;
                                        childUsersNotExists.Level3 = 0;
                                        childUsersNotExists.Level4 = 0;
                                        childUsersNotExists.Level5 = 0;
                                        childUsersNotExists.Level6 = 0;
                                        childUsersNotExists.Level7 = 0;
                                        childUsersNotExists.Level8 = 0;
                                        childUsersNotExists.Level9 = 0;
                                        childUsersNotExists.Level10 = 0;
                                        childUsersNotExists.Level1Users = 0;
                                        childUsersNotExists.Level2Users = 0;
                                        childUsersNotExists.Level3Users = 0;
                                        childUsersNotExists.Level4Users = 0;
                                        childUsersNotExists.Level5Users = 0;
                                        childUsersNotExists.Level6Users = 0;
                                        childUsersNotExists.Level7Users = 0;
                                        childUsersNotExists.Level8Users = 0;
                                        childUsersNotExists.Level9Users = 0;
                                        childUsersNotExists.Level10Users = 0;
                                        childUsersNotExists.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);

                                        entities.SaveChanges();
                                    }
                                }
                                //---------------- Make Transaction for Each user
                                var ud = entities.Users.FirstOrDefault(f => f.UserID == item.UserID);
                                var uniqueKey = $"{item.UserID}~{DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00).ToString("dd-MM-yyy")}~Earning";
                                var transaction = new Transaction()
                                {
                                    UserID = item.UserID,
                                    transactionDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                                    UniqueKey = uniqueKey,
                                    comment = "Level Earnings",
                                    paymentStatus = "Earning",
                                    amount = totalTransactionAmt,
                                    username = ud?.Name,
                                    mobilenumber = ud?.AspNetUser.UserName
                                };

                                entities.Transactions.Add(transaction);
                                entities.SaveChanges();

                                // -----Monthly Income Update
                                totalTransactionAmt = totalTransactionAmt * 30;
                                var UserMUpdate = entities.Users.Where(x => x.UserID == item.UserID).FirstOrDefault();
                                UserMUpdate.MothlyIncome = totalTransactionAmt;
                                entities.SaveChanges();
                            }

                            //Entery Update When Last Active Date User Not Active
                            else
                            {
                                var CNE = new LevelEarning();
                                var UserNotActive = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
                                //Entery Update
                                if (UserNotActive != null)
                                {
                                    UserNotActive.Level1 = 0;
                                    UserNotActive.Level2 = 0;
                                    UserNotActive.Level3 = 0;
                                    UserNotActive.Level4 = 0;
                                    UserNotActive.Level5 = 0;
                                    UserNotActive.Level6 = 0;
                                    UserNotActive.Level7 = 0;
                                    UserNotActive.Level8 = 0;
                                    UserNotActive.Level9 = 0;
                                    UserNotActive.Level10 = 0;
                                    UserNotActive.Level1Users = 0;
                                    UserNotActive.Level2Users = 0;
                                    UserNotActive.Level3Users = 0;
                                    UserNotActive.Level4Users = 0;
                                    UserNotActive.Level5Users = 0;
                                    UserNotActive.Level6Users = 0;
                                    UserNotActive.Level7Users = 0;
                                    UserNotActive.Level8Users = 0;
                                    UserNotActive.Level9Users = 0;
                                    UserNotActive.Level10Users = 0;
                                    UserNotActive.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);

                                    entities.SaveChanges();
                                }
                                //Entery Insert
                                else
                                {
                                    CNE.UserID = item.UserID;
                                    CNE.Level1 = 0;
                                    CNE.Level2 = 0;
                                    CNE.Level3 = 0;
                                    CNE.Level4 = 0;
                                    CNE.Level5 = 0;
                                    CNE.Level6 = 0;
                                    CNE.Level7 = 0;
                                    CNE.Level8 = 0;
                                    CNE.Level9 = 0;
                                    CNE.Level10 = 0;
                                    CNE.Level1Users = 0;
                                    CNE.Level2Users = 0;
                                    CNE.Level3Users = 0;
                                    CNE.Level4Users = 0;
                                    CNE.Level5Users = 0;
                                    CNE.Level6Users = 0;
                                    CNE.Level7Users = 0;
                                    CNE.Level8Users = 0;
                                    CNE.Level9Users = 0;
                                    CNE.Level10Users = 0;
                                    CNE.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                    entities.LevelEarnings.Add(CNE);
                                    entities.SaveChanges();
                                }
                            }
                        }
                        //Entery Update When Last Active Date User Not Active
                        else
                        {
                            var CNE = new LevelEarning();
                            var UserNotActive = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
                            //Entery Update
                            if (UserNotActive != null)
                            {
                                UserNotActive.Level1 = 0;
                                UserNotActive.Level2 = 0;
                                UserNotActive.Level3 = 0;
                                UserNotActive.Level4 = 0;
                                UserNotActive.Level5 = 0;
                                UserNotActive.Level6 = 0;
                                UserNotActive.Level7 = 0;
                                UserNotActive.Level8 = 0;
                                UserNotActive.Level9 = 0;
                                UserNotActive.Level10 = 0;
                                UserNotActive.Level1Users = 0;
                                UserNotActive.Level2Users = 0;
                                UserNotActive.Level3Users = 0;
                                UserNotActive.Level4Users = 0;
                                UserNotActive.Level5Users = 0;
                                UserNotActive.Level6Users = 0;
                                UserNotActive.Level7Users = 0;
                                UserNotActive.Level8Users = 0;
                                UserNotActive.Level9Users = 0;
                                UserNotActive.Level10Users = 0;
                                UserNotActive.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);

                                entities.SaveChanges();
                            }
                            //Entery Insert
                            else
                            {
                                CNE.UserID = item.UserID;
                                CNE.Level1 = 0;
                                CNE.Level2 = 0;
                                CNE.Level3 = 0;
                                CNE.Level4 = 0;
                                CNE.Level5 = 0;
                                CNE.Level6 = 0;
                                CNE.Level7 = 0;
                                CNE.Level8 = 0;
                                CNE.Level9 = 0;
                                CNE.Level10 = 0;
                                CNE.Level1Users = 0;
                                CNE.Level2Users = 0;
                                CNE.Level3Users = 0;
                                CNE.Level4Users = 0;
                                CNE.Level5Users = 0;
                                CNE.Level6Users = 0;
                                CNE.Level7Users = 0;
                                CNE.Level8Users = 0;
                                CNE.Level9Users = 0;
                                CNE.Level10Users = 0;
                                CNE.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                                entities.LevelEarnings.Add(CNE);
                                entities.SaveChanges();
                            }
                        }
                    }
                    return true;
                }
            }
            catch (Exception ex1)
            {
                MailSenderRepo mailSenderRepo = new MailSenderRepo();
                mailSenderRepo.MailSender("ssplyuvraj@gmail.com", ex1.Message + ex1.StackTrace, "Quiz Exception");
            }

            return false;
        }
        #endregion

        #region Get Level Wise User Information 
        public List<LevelWiseActiveUsers> GetLevelWiseUserInformation(LevelWiseModel model)
        {
            var activeUsers = entities.Users.Where(x => x.ParentIDs.Contains(model.UserId)).ToList();
            activeUsers = activeUsers.Where(x => x.LastActiveDate != null).ToList();
            activeUsers = activeUsers.Where(x =>  (x.LastActiveDate.Value).Date == (DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00).AddDays(-1)).Date || (x.LastActiveDate.Value).Date == (DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)).Date).ToList();
            List<LevelWiseActiveUsers> levelsUsers = new List<LevelWiseActiveUsers>();
            foreach (var childUsers in activeUsers)
            {
                int lvlCount = 0;
                string[] Sort = childUsers.ParentIDs.Split(',');
                foreach(var item in Sort)
                {
                    if(item != model.UserId)
                    {
                        lvlCount = lvlCount + 1;
                    }
                    else
                    {
                        lvlCount = lvlCount + 1;
                        break;
                    }
                }
                if (lvlCount == model.Level)
                {
                    var data = new LevelWiseActiveUsers();
                    var User = entities.AspNetUsers.Where(x => x.Id == childUsers.UserID).FirstOrDefault();
                    data.Name = childUsers.Name;
                    data.PhoneNumber = User.PhoneNumber;
                    levelsUsers.Add(data);
                }
            }
            return levelsUsers;
        }
        #endregion

        #region Top Ten Results on QuizId
        public List<TopResult> TopTenResultsonQuizId(QuizIDModel model)
        {
            var QuizPlayers = entities.QuizPlayers.Where(a => a.QuizID == model.QuizId && a.IsCompleted == true).OrderByDescending(x => x.PointEarn).Take(10).ToList();
            List<TopResult> TopTen = new List<TopResult>();

            //Sorting According To Consume Time
            for (int i = 0, j = 1; j < QuizPlayers.Count(); i++, j++)
            {
                if (QuizPlayers[i].PointEarn == QuizPlayers[j].PointEarn && QuizPlayers[i].TotalTimeTaken > QuizPlayers[j].TotalTimeTaken)
                {
                    var data = QuizPlayers[i];
                    QuizPlayers[i] = QuizPlayers[j];
                    QuizPlayers[j] = data;
                }
            }

            List<TopResult> TopTenUsers = new List<TopResult>();
            foreach (var Top in QuizPlayers)
            {

                var TopData = new TopResult();
                TopData.Name = entities.Users.Where(a => a.UserID == Top.UserID).Select(x => x.Name).FirstOrDefault();
                TopData.Score = (int)Top.PointEarn;
                TopData.Time = ((TimeSpan)Top.TotalTimeTaken).ToString(@"mm\:ss");
                TopData.status = Top.UserID == model.UserId ? true : false;
                TopTenUsers.Add(TopData);
            }



            return TopTenUsers;
        }
        #endregion

        #region Earning Heads
        public EaningHeadModel GetEaningHead()
        {
            EaningHeadModel earningHeads = new EaningHeadModel();
            var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
            }
            return earningHeads;
        }
        #endregion

        #region LevelFirebaseUpdates
        public void LevelFirebaseUpdates(bool status)
        {

            LevelEarningStatu obj = new LevelEarningStatu()
            {
                LevelHangFireStatus = status,
                StatusTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
            };
            entities.LevelEarningStatus.Add(obj);
            entities.SaveChanges();
            MailSenderRepo mailSenderRepo = new MailSenderRepo();
            mailSenderRepo.MailSender("ssplyuvraj@gmail.com", obj.LevelHangFireStatus.ToString(), "Quiz Exception");
        }
        #endregion

        #region Refresh Level Base Active Users and Monthly Income
        public bool RefreshLevelBaseActiveUsersandMonthlyIncome(string UserId)
        {
            try
            {
                var activeUser = entities.Users.Where(x => x.isActive == true).ToList();

                //Read Json File
                GeneralFunctions general = new GeneralFunctions();
                var earningHeads = general.getEarningHeads();
               
                double totalTransactionAmt = 0;
                List<LevelWithUser> lstLevelWithUser = new List<LevelWithUser>();

                //Get Child Users Where Current User
                var childUsers = activeUser.Where(x => !string.IsNullOrEmpty(x.ParentIDs) && x.ParentIDs.Split(',').Where(y => y == UserId).Any()).ToList();

                

                //childUsers = childUsers.Where(x => x.LastActiveDate != null && (x.LastActiveDate.Value).Date == (DateTime.Now.AddDays(-1)).Date || (x.LastActiveDate.Value).Date == (DateTime.Now).Date).ToList();

                totalTransactionAmt += earningHeads.DirectIncome/30;
                if (childUsers.Any())
                {
                    foreach (var level in childUsers)
                    {
                        if (level.LastActiveDate != null)
                        {
                            if ((level.LastActiveDate.Value).Date == DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00).AddDays(-1).Date || (level.LastActiveDate.Value).Date == (DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)).Date)
                            {
                                var parentIDs = level.ParentIDs.Split(',').ToList();
                                var parentUserWithLevel = parentIDs.Select(x => new SetLevelForParentUser()
                                {
                                    UserId = x,
                                    Level = parentIDs.IndexOf(x) + 1,
                                    Count = parentIDs.Count()
                                }).ToList();

                                //Add Level 
                                var level_1 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 1).FirstOrDefault();
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

                                var level_2 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 2).FirstOrDefault();
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

                                var level_3 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 3).FirstOrDefault();
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

                                var level_4 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 4).FirstOrDefault();
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

                                var level_5 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 5).FirstOrDefault();
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

                                var level_6 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 6).FirstOrDefault();
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

                                var level_7 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 7).FirstOrDefault();
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

                                var level_8 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 8).FirstOrDefault();
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

                                var level_9 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 9).FirstOrDefault();
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

                                var level_10 = parentUserWithLevel.Where(x => x.UserId == UserId && x.Level == 10).FirstOrDefault();
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
                        }
                    }

                    //Add Actual Earning Based on Level
                    if (lstLevelWithUser.Any())
                    {
                        var le = new LevelEarning();
                        GeneralFunctions generalFunctions = new GeneralFunctions();
                        var userEarningExist = entities.LevelEarnings.Where(x => x.UserID == UserId).FirstOrDefault();
                        foreach (var lst in lstLevelWithUser)
                        {
                            int userCount = 0; double actualEarning = 0;

                            //Already this User Exist check Condition then Update
                            if (userEarningExist != null)
                            {



                                if (lst.Level == 1)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    userEarningExist.Level1Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 2)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level2Income / 30)), 2);
                                    userEarningExist.Level2Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 3)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level3Income / 30)), 2);
                                    userEarningExist.Level3Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 4)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level4Income / 30)), 2);
                                    userEarningExist.Level4Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 5)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level5Income / 30)), 2);
                                    userEarningExist.Level5Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 6)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level6Income / 30)), 2);
                                    userEarningExist.Level6Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 7)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level7Income / 30)), 2);
                                    userEarningExist.Level7Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 8)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level8Income / 30)), 2);
                                    userEarningExist.Level8Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 9)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level9Income / 30)), 2);
                                    userEarningExist.Level9Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 10)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level10Income / 30)), 2);
                                    userEarningExist.Level10Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }
                            }

                            else
                            {
                                //If Not Exist then Insert 
                                le.UserID = UserId;
                                if (lst.Level == 1)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level1Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 2)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level2Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }


                                if (lst.Level == 3)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level3Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 4)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level4Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 5)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level5Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 6)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level6Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 7)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level7Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 8)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level8Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 9)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
                                    le.Level9Users = userCount;
                                    totalTransactionAmt += actualEarning;
                                }

                                if (lst.Level == 10)
                                {
                                    userCount = lst.ChildUsers.Count();
                                    actualEarning = Math.Round((userCount * (earningHeads.Level1Income / 30)), 2);
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
                    else
                    {
                        var le = new LevelEarning();
                        var userEarningExist = entities.LevelEarnings.Where(x => x.UserID == UserId).FirstOrDefault();
                        if (userEarningExist == null)
                        {
                            le.Level1 = 0;
                            le.Level2 = 0;
                            le.Level3 = 0;
                            le.Level4 = 0;
                            le.Level5 = 0;
                            le.Level6 = 0;
                            le.Level7 = 0;
                            le.Level8 = 0;
                            le.Level9 = 0;
                            le.Level10 = 0;
                            le.Level1Users = 0;
                            le.Level2Users = 0;
                            le.Level3Users = 0;
                            le.Level4Users = 0;
                            le.Level5Users = 0;
                            le.Level6Users = 0;
                            le.Level7Users = 0;
                            le.Level8Users = 0;
                            le.Level9Users = 0;
                            le.Level10Users = 0;
                            entities.LevelEarnings.Add(le);
                        }
                        else
                        {
                            userEarningExist.Level1 = 0;
                            userEarningExist.Level2 = 0;
                            userEarningExist.Level3 = 0;
                            userEarningExist.Level4 = 0;
                            userEarningExist.Level5 = 0;
                            userEarningExist.Level6 = 0;
                            userEarningExist.Level7 = 0;
                            userEarningExist.Level8 = 0;
                            userEarningExist.Level9 = 0;
                            userEarningExist.Level10 = 0;
                            userEarningExist.Level1Users = 0;
                            userEarningExist.Level2Users = 0;
                            userEarningExist.Level3Users = 0;
                            userEarningExist.Level4Users = 0;
                            userEarningExist.Level5Users = 0;
                            userEarningExist.Level6Users = 0;
                            userEarningExist.Level7Users = 0;
                            userEarningExist.Level8Users = 0;
                            userEarningExist.Level9Users = 0;
                            userEarningExist.Level10Users = 0;
                        }
                        entities.SaveChanges();
                    }
                }
                //when Child Users not Exists of activeUsers 
                else
                {
                    var CNE = new LevelEarning();
                    var childUsersNotExists = entities.LevelEarnings.Where(x => x.UserID == UserId).FirstOrDefault();
                    //Entery Update When Active Users Already Existes in Level Earning Table
                    if (childUsersNotExists == null)
                    {
                        CNE.UserID = UserId;
                        CNE.Level1 = 0;
                        CNE.Level2 = 0;
                        CNE.Level3 = 0;
                        CNE.Level4 = 0;
                        CNE.Level5 = 0;
                        CNE.Level6 = 0;
                        CNE.Level7 = 0;
                        CNE.Level8 = 0;
                        CNE.Level9 = 0;
                        CNE.Level10 = 0;
                        CNE.Level1Users = 0;
                        CNE.Level2Users = 0;
                        CNE.Level3Users = 0;
                        CNE.Level4Users = 0;
                        CNE.Level5Users = 0;
                        CNE.Level6Users = 0;
                        CNE.Level7Users = 0;
                        CNE.Level8Users = 0;
                        CNE.Level9Users = 0;
                        CNE.Level10Users = 0;
                        CNE.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                        entities.LevelEarnings.Add(CNE);
                        entities.SaveChanges();
                    }
                    //Entery Insert When Active Users Already not Existes in Level Earning Table
                    else
                    {
                        childUsersNotExists.Level1 = 0;
                        childUsersNotExists.Level2 = 0;
                        childUsersNotExists.Level3 = 0;
                        childUsersNotExists.Level4 = 0;
                        childUsersNotExists.Level5 = 0;
                        childUsersNotExists.Level6 = 0;
                        childUsersNotExists.Level7 = 0;
                        childUsersNotExists.Level8 = 0;
                        childUsersNotExists.Level9 = 0;
                        childUsersNotExists.Level10 = 0;
                        childUsersNotExists.Level1Users = 0;
                        childUsersNotExists.Level2Users = 0;
                        childUsersNotExists.Level3Users = 0;
                        childUsersNotExists.Level4Users = 0;
                        childUsersNotExists.Level5Users = 0;
                        childUsersNotExists.Level6Users = 0;
                        childUsersNotExists.Level7Users = 0;
                        childUsersNotExists.Level8Users = 0;
                        childUsersNotExists.Level9Users = 0;
                        childUsersNotExists.Level10Users = 0;
                        childUsersNotExists.LastUpdate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);

                        entities.SaveChanges();
                    }
                }
                // -----Monthly Income Update
                    totalTransactionAmt = totalTransactionAmt * 30;
                var UserMUpdate = entities.Users.Where(x => x.UserID == UserId).FirstOrDefault();
                UserMUpdate.MothlyIncome = totalTransactionAmt;
                entities.SaveChanges();
            }
            catch (Exception ex1)
            {
                var data = ex1.Message;
            }

            return true;
        }
        #endregion

        #region Get EarningHead Model
        public EaningHeadModel GetEarningHeadModel()
        {
            var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
            EaningHeadModel earningHeads = new EaningHeadModel();
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
            }
            return earningHeads;
        }
        #endregion

        #region Contact Support
        public bool ContactSupport(ContactSupportModel model)
        {
            entities.ContactSupports.Add(new ContactSupport() {
                UserId=model.UserId,
                Mobile=model.Mobile,
                UserMessage=model.UserMessage,
                CreateDate= DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
            });
            return entities.SaveChanges() > 0;
        }
        #endregion

        #region Check Max Quiz Played
        public bool CheckMaxQuizPlayed(string userId)
        {
            bool res = false;
            var playedDataCount = entities.QuizPlayers.Where(a => a.UserID == userId && a.IsCompleted == true && a.PlayedDate != null).OrderByDescending(a => a.PlayedDate).ToList();
            //var d = DateTime.Now.Date;
            playedDataCount = playedDataCount.Where(a => (a.PlayedDate.Value).Date == (DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)).Date).OrderByDescending(a => a.PlayedDate).ToList();
            var eaningHeadModel = GetEaningHead();
            int Count = playedDataCount.Count();
            if (Count < eaningHeadModel.MaxQuiz)
            {
                res = true;
            }
            return res;
        }
        #endregion
    }
}