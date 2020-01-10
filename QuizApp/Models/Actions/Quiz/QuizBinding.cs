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
            LevelEarningModelMaster levelEarningModelMaster = new LevelEarningModelMaster();
            var data = entities.LevelEarnings.Where(x => x.UserID == Users).FirstOrDefault();
            List<LevelEarning> levelEarnings = new List<LevelEarning>();
            for (int i = 1; i <= 10; i++)
            {
                var lvl = new LevelEarning();
                switch (i)
                {
                    case 1:
                        lvl.Title = "Level 1";
                        lvl.Activeuser = data!=null ? data.Level1Users != null ? (int)data.Level1Users : 0 : 0;
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
        public void ADDLevelBaseEarningAmount()
        {
            //var activeUsers = entities.Users.Where(x => x.isActive == true).ToList();
            //if (activeUsers.Any())
            //{
            //    EaningHeadModel earningHeads = new EaningHeadModel();
            //    var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
            //    using (StreamReader r = new StreamReader(jsonFilePath))
            //    {
            //        string json = r.ReadToEnd();
            //        earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
            //    }
            //    foreach (var item in activeUsers)
            //    {
            //        List<LevelWithUser> lstLevelWithUser = new List<LevelWithUser>();
            //        var childUsers = activeUsers.Where(x => x.ParentIDs.Contains(item.UserID) && x.LastActiveDate == DateTime.Now.AddDays(-1)).ToList();
            //        if (childUsers.Any())
            //        {
            //            foreach (var level in childUsers)
            //            {
            //                var parentIDs = level.ParentIDs.Split(',').ToList();
            //                var parentUserWithLevel = parentIDs.Select(x => new SetLevelForParentUser()
            //                {
            //                    UserId = x,
            //                    Level = parentIDs.IndexOf(x) + 1,
            //                    Count = parentIDs.Count()
            //                }).ToList();

            //                var level_1 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 1).FirstOrDefault();
            //                if (level_1 != null)
            //                {
            //                    LevelWithUser l1 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 1).Any())
            //                    {
            //                        l1 = new LevelWithUser();
            //                        l1.Level = 1;
            //                        l1.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l1);
            //                    }
            //                    l1.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_2 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 2).FirstOrDefault();
            //                if (level_2 != null)
            //                {
            //                    LevelWithUser l2 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 2).Any())
            //                    {
            //                        l2 = new LevelWithUser();
            //                        l2.Level = 2;
            //                        l2.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l2);
            //                    }
            //                    l2.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_3 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 3).FirstOrDefault();
            //                if (level_3 != null)
            //                {
            //                    LevelWithUser l3 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 3).Any())
            //                    {
            //                        l3 = new LevelWithUser();
            //                        l3.Level = 3;
            //                        l3.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l3);
            //                    }
            //                    l3.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_4 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 4).FirstOrDefault();
            //                if (level_4 != null)
            //                {
            //                    LevelWithUser l4 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 4).Any())
            //                    {
            //                        l4 = new LevelWithUser();
            //                        l4.Level = 4;
            //                        l4.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l4);
            //                    }
            //                    l4.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_5 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 5).FirstOrDefault();
            //                if (level_5 != null)
            //                {
            //                    LevelWithUser l5 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 5).Any())
            //                    {
            //                        l5 = new LevelWithUser();
            //                        l5.Level = 5;
            //                        l5.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l5);
            //                    }
            //                    l5.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_6 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 6).FirstOrDefault();
            //                if (level_6 != null)
            //                {
            //                    LevelWithUser l6 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 6).Any())
            //                    {
            //                        l6 = new LevelWithUser();
            //                        l6.Level = 6;
            //                        l6.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l6);
            //                    }
            //                    l6.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_7 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 7).FirstOrDefault();
            //                if (level_7 != null)
            //                {
            //                    LevelWithUser l7 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 7).Any())
            //                    {
            //                        l7 = new LevelWithUser();
            //                        l7.Level = 7;
            //                        l7.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l7);
            //                    }
            //                    l7.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_8 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 8).FirstOrDefault();
            //                if (level_8 != null)
            //                {
            //                    LevelWithUser l8 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 8).Any())
            //                    {
            //                        l8 = new LevelWithUser();
            //                        l8.Level = 8;
            //                        l8.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l8);
            //                    }
            //                    l8.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_9 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 9).FirstOrDefault();
            //                if (level_9 != null)
            //                {
            //                    LevelWithUser l9 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 9).Any())
            //                    {
            //                        l9 = new LevelWithUser();
            //                        l9.Level = 9;
            //                        l9.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l9);
            //                    }
            //                    l9.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                var level_10 = parentUserWithLevel.Where(x => x.UserId == item.UserID && x.Level == 10).FirstOrDefault();
            //                if (level_10 != null)
            //                {
            //                    LevelWithUser l10 = null;
            //                    if (!lstLevelWithUser.Where(x => x.Level == 10).Any())
            //                    {
            //                        l10 = new LevelWithUser();
            //                        l10.Level = 10;
            //                        l10.ChildUsers = new List<ChildUser>();
            //                        lstLevelWithUser.Add(l10);
            //                    }
            //                    l10.ChildUsers.Add(new ChildUser()
            //                    {
            //                        UserId = level.UserID
            //                    });
            //                }

            //                //string[] s = level.ParentIDs.Split(',');
            //                //if(s.Length==1)
            //                //{
            //                //    int data = 1;
            //                //}
            //            }
            //        }

            //        if (lstLevelWithUser.Any())
            //        {
            //            double totalTransactionAmt = 0;
            //            var userEarningExist = entities.LevelEarnings.Where(x => x.UserID == item.UserID).FirstOrDefault();
            //            foreach (var lst in lstLevelWithUser)
            //            {
            //                var le = new LevelEarning();
            //                int userCount = 0; double actualEarning = 0;
            //                if (userEarningExist != null)
            //                {
            //                    if (lst.Level == 1)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level1Income / 30);
            //                        userEarningExist.Level1 = actualEarning;
            //                        userEarningExist.Level1Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 2)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level2Income / 30);
            //                        userEarningExist.Level2 = actualEarning;
            //                        userEarningExist.Level2Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 3)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level3Income / 30);
            //                        userEarningExist.Level3 = actualEarning;
            //                        userEarningExist.Level3Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 4)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level4Income / 30);
            //                        userEarningExist.Level4 = actualEarning;
            //                        userEarningExist.Level4Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 5)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level5Income / 30);
            //                        userEarningExist.Level5 = actualEarning;
            //                        userEarningExist.Level5Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 6)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level6Income / 30);
            //                        userEarningExist.Level6 = actualEarning;
            //                        userEarningExist.Level6Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 7)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level7Income / 30);
            //                        userEarningExist.Level7 = actualEarning;
            //                        userEarningExist.Level7Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 8)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level8Income / 30);
            //                        userEarningExist.Level8 = actualEarning;
            //                        userEarningExist.Level8Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 9)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level9Income / 30);
            //                        userEarningExist.Level9 = actualEarning;
            //                        userEarningExist.Level9Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 10)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level10Income / 30);
            //                        userEarningExist.Level10 = actualEarning;
            //                        userEarningExist.Level10Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    entities.SaveChanges();
            //                }
            //                else
            //                {
            //                    le.UserID = item.UserID;
            //                    if (lst.Level == 1)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level1Income / 30);
            //                        le.Level1 = actualEarning;
            //                        le.Level1Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 2)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level2Income / 30);
            //                        le.Level2 = actualEarning;
            //                        le.Level2Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 3)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level3Income / 30);
            //                        le.Level3 = actualEarning;
            //                        le.Level3Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 4)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level4Income / 30);
            //                        le.Level4 = actualEarning;
            //                        le.Level4Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 5)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level5Income / 30);
            //                        le.Level5 = actualEarning;
            //                        le.Level5Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 6)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level6Income / 30);
            //                        le.Level6 = actualEarning;
            //                        le.Level6Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 7)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level7Income / 30);
            //                        le.Level7 = actualEarning;
            //                        le.Level7Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 8)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level8Income / 30);
            //                        le.Level8 = actualEarning;
            //                        le.Level8Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 9)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level9Income / 30);
            //                        le.Level9 = actualEarning;
            //                        le.Level9Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }
            //                    if (lst.Level == 10)
            //                    {
            //                        userCount = lst.ChildUsers.Count();
            //                        actualEarning = userCount * (earningHeads.Level10Income / 30);
            //                        le.Level10 = actualEarning;
            //                        le.Level10Users = userCount;
            //                        totalTransactionAmt += actualEarning;
            //                    }

            //                    entities.LevelEarnings.Add(le);
            //                    entities.SaveChanges();
            //                }
            //            }

            //            // totalTransactionAmt
            //            // Make Entry for Transacation
            //        }
            //    }
            //}
            //else
            //{
            //    return null;
            //}
            //return string.Empty;
        }
        #endregion
    }
}