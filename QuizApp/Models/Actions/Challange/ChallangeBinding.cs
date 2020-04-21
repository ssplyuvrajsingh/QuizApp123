using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using QuizApp.Models.Entities;

namespace QuizApp.Models
{
    public class ChallangeBinding
    {
        #region Database Entities Declaration
        QuizAppEntities entities = new QuizAppEntities();
        #endregion

        #region First Screen Create Challange
        public ChallangeIdModel ChallangePost(ChallangeModel model)
        {
            var Challange = new Entities.Challange()
            {
                UserId = model.UserId,
                Name = model.Name,
                Phone = model.Phone,
                IsAdmin = true,
                IsAccepted = true,
                StartDateTime = DateTime.Now,
                IsCompleted = false,
                CompletedDateTime = null,
                IsWinner = false,
                Points = 0
            };
            entities.Challanges.Add(Challange);
            entities.SaveChanges();
            int ChallangeId = Challange.Id;
            var data = entities.Challanges.Where(x => x.Id == ChallangeId).FirstOrDefault();
            if (data != null)
            {
                data.ChallangeId = ChallangeId;
                entities.SaveChanges();
            }
            return new ChallangeIdModel()
            {
                ChallangeId = data.Id
            };
        }
        #endregion

        #region Second Screen Search Users
        public List<SearchModel> SearcUser(SearchKeyModel model)
        {
            var data = (from User in entities.Users
                        join Asp in entities.AspNetUsers on User.UserID equals Asp.Id where  User.Name.Contains(model.Search) || Asp.PhoneNumber.Contains(model.Search)
                        select new SearchModel {
                            UserID = User.UserID,
                            Name = User.Name,
                            Phone = Asp.PhoneNumber
                        }).ToList();
            List<SearchModel> searchModels = new List<SearchModel>();
            foreach(var item in data)
            {
                var Challangers = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.UserId == item.UserID).FirstOrDefault();
                var Search = new SearchModel();
                Search.UserID = item.UserID;
                Search.Name = item.Name;
                Search.Phone = item.Phone;
                if(Challangers!=null)
                {
                    Search.Status = true;
                }
                else
                {
                    Search.Status = false;
                }
                searchModels.Add(Search);
            }
            return searchModels;
        }
        #endregion

        #region Third Screen Add User for Challange
        public List<SearchModel> AddUserChallange(ChallangeModel model)
        {
            var data = new Challange()
            {
                UserId = model.UserId,
                Name = model.Name,
                Phone = model.Phone,
                ChallangeId = model.ChallangeId,
                IsAdmin = false,
                IsAccepted = true,
                StartDateTime = DateTime.Now,
                IsCompleted = false,
                CompletedDateTime = null,
                IsWinner = false,
                Points = 0
            };
            entities.Challanges.Add(data);
            entities.SaveChanges();
           var ChallangesListModel = AddedChallangersList(new ChallangeIdModel() { 
           ChallangeId = model.ChallangeId
           });
           return ChallangesListModel;
        }
        public List<SearchModel> AddedChallangersList(ChallangeIdModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).ToList();
            List<SearchModel> ChallangesListModel = new List<SearchModel>();
            foreach (var item in data)
            {
                var Challangers = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.UserId == item.UserId).FirstOrDefault();
                var CLD = new SearchModel();
                CLD.UserID = item.UserId;
                CLD.Name = item.Name;
                CLD.Phone = item.Phone;
                if (Challangers != null)
                {
                    CLD.Status = true;
                }
                else
                {
                    CLD.Status = false;
                }
                ChallangesListModel.Add(CLD);
            }
            return ChallangesListModel;
        }
        #endregion

        #region Fourth Screen Add Invest Point for Challange  
        public bool AddInvestPoint(ChallangeModel model)
        {
            var res = false;
            var User= entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if (User.CurrentPoint >= model.Points && User.CurrentPoint > 0)
            {
                var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId).OrderByDescending(x => x.StartDateTime).FirstOrDefault();
                if (data != null)
                {
                    data.Points = model.Points;
                    entities.SaveChanges();
                    res = true;
                }
            }
            return res;
        }
        #endregion

        #region Five Screen Get Challangers List
        public List<ChallangesListModel> GetChallangersList(ChallangeIdModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).ToList();
            List<ChallangesListModel> challangesListModels = new List<ChallangesListModel>();
            foreach(var item in data)
            {
                var CLD = new ChallangesListModel();
                CLD.UserId = item.UserId;
                CLD.ChallangeId = item.ChallangeId;
                CLD.Name = item.Name;
                CLD.Phone = item.Phone;
                CLD.IsAdmin = item.IsAdmin;
                CLD.IsAccepted = item.IsAccepted;
                CLD.Points = item.Points;
                challangesListModels.Add(CLD);
            }
            return challangesListModels;
        }
        #endregion

        #region Set Challange Result
        public bool SetChallangeResult(ChallangeModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).ToList();

            var WinUser = data.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId).FirstOrDefault();

            int Points = 0;
            foreach(var item in data)
            {
                if(item.UserId != model.UserId && item.ChallangeId == model.ChallangeId)
                {
                    item.IsCompleted = true;
                    item.CompletedDateTime = DateTime.Now;
                    item.IsWinner = false;
                    Points = Convert.ToInt32(Points + item.Points);
                    UserPoint PointWithdrawal = new UserPoint()
                    {
                        UserID = item.UserId,
                        TransactionDate = DateTime.Now,
                        PointsWithdraw = item.Points,
                        PointsEarned = 0,
                        Description = "Point Withdrawal for Lose ChallangeId= "+model.ChallangeId,
                        CreatedDate = DateTime.Now
                    };
                    entities.UserPoints.Add(PointWithdrawal);
                    entities.SaveChanges();
                }
            }

            WinUser.IsCompleted = true;
            WinUser.CompletedDateTime = DateTime.Now;
            WinUser.IsWinner = true;
            WinUser.Points = Points;
            UserPoint Point = new UserPoint()
            {
                UserID = model.UserId,
                TransactionDate = DateTime.Now,
                PointsWithdraw = 0,
                PointsEarned = Points,
                Description = "Point Earn for Win ChallangeId= " + model.ChallangeId,
                CreatedDate = DateTime.Now
            };
            entities.UserPoints.Add(Point);
            entities.SaveChanges();
            return true;
        }
        #endregion

        #region Rewards Request for Points
        public bool RewardsPoints(UserModel model)
        {
            GeneralFunctions generalFunctions = new GeneralFunctions();
            var earning = generalFunctions.getEarningHeads();
            UserPoint Point = new UserPoint()
            {
                UserID = model.UserId,
                TransactionDate = DateTime.Now,
                PointsWithdraw = 0,
                PointsEarned = earning.Rewards,
                Description = "Point Earn Rewards",
                CreatedDate = DateTime.Now
            };
            entities.UserPoints.Add(Point);
            return entities.SaveChanges() > 0;
        }
        #endregion

        #region Winner User List
        public List<WinnerUsersModel> WinnerUsersList()
        {
            var data = entities.Challanges.Where(x => x.IsWinner == true).OrderByDescending(x => x.CompletedDateTime).ToList();
            List<WinnerUsersModel> winnerUsers = new List<WinnerUsersModel>();
            foreach (var item in data)
            {
                var Win = new WinnerUsersModel();
                Win.UserId = item.UserId;
                Win.Name = item.Name;
                Win.ChallangeId = (int)item.ChallangeId;
                Win.WinDate = string.Format("{0:dd MMMM, yyyy}", item.CompletedDateTime);
                winnerUsers.Add(Win);
            }
            return winnerUsers;
        }
        #endregion

        #region Winner User List
        public WinnerDetailsModel WinnerUserDetails(WinnerUsersModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId).Select(x => new WinnerDetailsModel()
            {
                Name = x.Name,
                Phone = x.Phone,
                Points = (int)x.Points
            }).FirstOrDefault();
            return data;
        }
        #endregion
    }
}