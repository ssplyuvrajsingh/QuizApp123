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
        public bool ChallangeAccept(ChallangeModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId).FirstOrDefault();
            if(data!=null)
            {
                data.IsAccepted = model.IsAccepted;
                return entities.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
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
        public ChallangeListsModel GetChallangersList(ChallangeIdModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).ToList();
            ChallangeListsModel challangeListsModel = new ChallangeListsModel();
            List<ChallangesListModel> challangesListModels = new List<ChallangesListModel>();
            int TotalPoints = 0;
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
                TotalPoints = Convert.ToInt32(TotalPoints + item.Points);
                challangesListModels.Add(CLD);
            }
            GeneralFunctions general = new GeneralFunctions();
            EaningHeadModel Challangers = new EaningHeadModel();
            Challangers = general.getEarningHeads();
            challangeListsModel.challangesLists = challangesListModels;
            challangeListsModel.TotalPoints = TotalPoints;
            challangeListsModel.MinimumChallangerUsers = Challangers.MinimumChallangerUsers;
            return challangeListsModel;
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

        #region Winner User Details
        public List<WinnerDetailsModel> WinnerUserDetails(WinnerUsersModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).Select(x => new WinnerDetailsModel()
            {
                Name = x.Name,
                Phone = x.Phone,
                Points = (int)x.Points,
                IsWinner = x.IsWinner
            }).ToList();
            return data;
        }
        #endregion

        #region Get Challange Ids for Challange Notification
        public List<string> GetNotificationForChallange(UserModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.IsAccepted.ToString() == string.Empty).OrderByDescending(x => x.StartDateTime).ToList();
            var ChallangeIds = new List<string>();
            foreach(var item in data)
            {
                ChallangeIds.Add(item.ChallangeId.ToString());
            }
            return ChallangeIds;
        }
        #endregion

        #region Get Challange List created by User
        public List<SavedChallangeModel> SaveChallangeList(UserModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.IsCompleted == false && x.IsAdmin == true).OrderByDescending(x => x.StartDateTime).ToList();
            List<SavedChallangeModel> saveds = new List<SavedChallangeModel>();
           foreach(var item in data)
            {
                var List = new SavedChallangeModel();
                
                List.ChallangeId = (int)item.ChallangeId;
                List.StartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", item.StartDateTime);
                List.UserId = item.UserId;
                saveds.Add(List);

                // Send request for challange get list here
                
                var res = entities.Challanges.Where(x => x.IsCompleted == false && x.IsAdmin == false && x.ChallangeId == item.ChallangeId).OrderByDescending(x => x.StartDateTime).FirstOrDefault();
                if (res != null)
                {
                    var List1 = new SavedChallangeModel();
                    List1.ChallangeId = (int)res.ChallangeId;
                    List1.StartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", res.StartDateTime);
                    List1.UserId = res.UserId;
                    saveds.Add(List1);
                }
            }
            return saveds;
        }
        #endregion

        #region Get Challange List come request for challange by Other User
        public List<SavedChallangeModel> RequestChallangeList(UserModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.IsCompleted == false && x.IsAdmin == false).OrderByDescending(x => x.StartDateTime).ToList();
            List<SavedChallangeModel> saveds = new List<SavedChallangeModel>();
            foreach (var item in data)
            {
                var res = entities.Challanges.Where(x =>x.IsCompleted == false && x.IsAdmin == true && x.ChallangeId == item.ChallangeId).OrderByDescending(x => x.StartDateTime).FirstOrDefault();
                var List = new SavedChallangeModel();
                List.ChallangeId = (int)res.ChallangeId;
                List.StartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", res.StartDateTime);
                List.UserId = res.UserId;
                saveds.Add(List);
            }
            return saveds;
        }
        #endregion
    }
}