using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
                StartDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
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
        public bool AddUserChallange(ChallangeModel model)
        {
            var data = new Challange()
            {
                UserId = model.UserId,
                Name = model.Name,
                Phone = model.Phone,
                ChallangeId = model.ChallangeId,
                IsAdmin = false,
                StartDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                IsCompleted = false,
                CompletedDateTime = null,
                IsWinner = false,
                Points = 0
            };
            entities.Challanges.Add(data);
            return entities.SaveChanges() > 0;
        }
        public AddUserModel AddedUserList(ChallangeModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId).ToList();
            AddUserModel addUser = new AddUserModel();
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
            GeneralFunctions general = new GeneralFunctions();
            EaningHeadModel eaningHead = new EaningHeadModel();
            eaningHead = general.getEarningHeads();
            addUser.AddUsers = ChallangesListModel;
            addUser.MinimumChallangerUsers = eaningHead.MinimumChallangerUsers;
            return addUser;
        }
        public bool ChallangeAccept(ChallangeModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId).FirstOrDefault();
            var Admin = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAdmin == true).FirstOrDefault();
            if (data!=null)
            {
                data.IsAccepted = model.IsAccepted;
                data.ChallangeStartDateTime = Admin.ChallangeStartDateTime;
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
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId).ToList();
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
                if((bool)item.IsAdmin)
                {
                    challangeListsModel.AdminPoints = (int)item.Points;
                    challangeListsModel.MinimumEntryPoints = (int)item.MinimumEntryPoints;
                }
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
                    item.CompletedDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                    item.IsWinner = false;
                    Points = Convert.ToInt32(Points + item.Points);
                    UserPoint PointWithdrawal = new UserPoint()
                    {
                        UserID = item.UserId,
                        TransactionDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                        PointsWithdraw = item.Points,
                        PointsEarned = 0,
                        Description = "Point Withdrawal for Lose ChallangeId= "+model.ChallangeId,
                        CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
                    };
                    entities.UserPoints.Add(PointWithdrawal);
                    entities.SaveChanges();
                }
            }

            WinUser.IsCompleted = true;
            WinUser.CompletedDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
            WinUser.IsWinner = true;
            WinUser.Points = Points;
            UserPoint Point = new UserPoint()
            {
                UserID = model.UserId,
                TransactionDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                PointsWithdraw = 0,
                PointsEarned = Points,
                Description = "Point Earn for Win ChallangeId= " + model.ChallangeId,
                CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
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
                TransactionDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00),
                PointsWithdraw = 0,
                PointsEarned = earning.Rewards,
                Description = "Point Earn Rewards",
                CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
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
                
                //var res = entities.Challanges.Where(x => x.IsCompleted == false && x.IsAdmin == false && x.ChallangeId == item.ChallangeId).OrderByDescending(x => x.StartDateTime).ToList();
                //foreach (var sendRequest in res)
                //{
                //    var List1 = new SavedChallangeModel();
                //    List1.ChallangeId = (int)sendRequest.ChallangeId;
                //    List1.StartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", sendRequest.StartDateTime);
                //    List1.UserId = sendRequest.UserId;
                //    saveds.Add(List1);
                //}
            }
            return saveds;
        }
        #endregion

        #region Get Challange List come request for challange by Other User
        public List<RequestChallangeModel> RequestChallangeList(UserModel model)
        {
            var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.IsCompleted == false && x.IsAdmin == false).OrderByDescending(x => x.StartDateTime).ToList();
            List<RequestChallangeModel> saveds = new List<RequestChallangeModel>();
            foreach (var item in data)
            {
                var res = entities.Challanges.Where(x =>x.IsCompleted == false && x.IsAdmin == true && x.ChallangeId == item.ChallangeId).OrderByDescending(x => x.StartDateTime).FirstOrDefault();
                var List = new RequestChallangeModel();
                if (res != null)
                {
                    List.ChallangeId = (int)res.ChallangeId;
                    List.StartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", res.StartDateTime);
                    List.UserId = res.UserId;
                    List.Name = res.Name;
                    saveds.Add(List);
                }
            }
            return saveds;
        }
        #endregion

        #region Delete Saved Challenge
        public bool DeleteSavedChallenge(ChallangeIdModel model)
        {
            var data = entities.Database.ExecuteSqlCommand("Delete Challange where ChallangeId=" + model.ChallangeId);
            if (data > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region set Temporary Winner
        public WinnerUserModel SetTemporaryWinner(ChallangeIdModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAccepted == true).ToList();
           

            var TempWinUser = data.Where(x => x.TemporaryWinner == true).FirstOrDefault();
            if (TempWinUser == null)
            {
                string RandomUser = GeneralFunctions.GetRandomTemporaryUser(data.Count());
                int count = 0;
                foreach (var item in data)
                {
                    if (count == Convert.ToInt32(RandomUser))
                    {
                        item.TemporaryWinner = true;
                    }
                    else
                    {
                        item.TemporaryWinner = false;
                    }
                    entities.SaveChanges();
                    count++;
                }         
            }
            TempWinUser = data.Where(x => x.TemporaryWinner == true).FirstOrDefault();
            return new WinnerUserModel() {
                Phone = TempWinUser.Phone
            };
        }
        #endregion

        #region Get Challenge Time
        public WinnerUserModel GetChallengeTime(ChallangeIdModel model)
        {
            var WinUser= entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsWinner == true).FirstOrDefault();
            if (WinUser == null)
            {
                var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.IsAdmin == true).FirstOrDefault();

                if (data != null)
                {
                    return new WinnerUserModel()
                    {
                        ChallangeStartDateTime = string.Format("{0:MMMM dd, yyyy HH:mm:ss}", data.ChallangeStartDateTime),
                        IsStatus = false
                    };
                }
                else
                {
                    return null;
                }
            }
            return new WinnerUserModel()
            {
                ChallangeStartDateTime = string.Format("{0:MMMM dd, yyyy HH:mm:ss}", WinUser.ChallangeStartDateTime),
                IsStatus = true,
                Phone = WinUser.Phone,
                Name = WinUser.Name
            };
        }
        #endregion

        #region Set Winner User
        public WinnerUserModel SetWinnerUser(ChallangeIdModel model)
        {
            var data = entities.Challanges.Where(x => x.ChallangeId == model.ChallangeId && x.TemporaryWinner == true).FirstOrDefault();
            var winner = new WinnerUserModel();
            if (data.IsWinner != true)
            {
                SetChallangeResult(new ChallangeModel()
                {
                    UserId = data.UserId,
                    ChallangeId = data.ChallangeId
                });
                winner.IsStatus = true;
            }

            winner.ChallangeStartDateTime = string.Format("{0:dd MMMM, yyyy HH:mm:ss}", data.ChallangeStartDateTime);
            winner.Phone = data.Phone;
            winner.Name = data.Name;
            return winner;

        }
        #endregion

        #region Save Minimum Entry Points
        public bool SaveMinimumEntryPoints(ChallangeModel model)
        {
            bool res = false;
                var data = entities.Challanges.Where(x => x.UserId == model.UserId && x.ChallangeId == model.ChallangeId && x.IsAdmin == true).OrderByDescending(x => x.StartDateTime).FirstOrDefault();
                if (data != null)
                {
                    data.MinimumEntryPoints = model.Points;
                    entities.SaveChanges();
                    res = true;
                }
            
            return res;
        }
        #endregion

        #region Send Challenge Start Notification
        public bool SendChallengeStartNotification(ChallangeIdModel model)
        {
            bool res = false;
            var data = entities.Challanges.Where(x => x.IsAccepted == true && x.ChallangeId == model.ChallangeId).ToList();
            foreach(var item in data)
            {
                var FCM = entities.Users.Where(x => x.UserID == item.UserId).FirstOrDefault();
                if (FCM.FCMToken != null && item.IsAdmin != true)
                {
                    string ChallengeDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", item.StartDateTime);
                    var result = FCMPushNotification.SendNotificationFromFirebaseCloud(FCM.FCMToken, ChallengeDateTime);
                    if (result.success == 1)
                    {
                        res = true;
                    }
                    else
                    {
                        res = false;
                        break;
                    }
                }
            }
            return res;
        }
        #endregion

        #region Challenge Starting Soon
        public List<ChallengeStartSoonModel> ChallengeStartingSoon(UserModel model)
        {
            var data = entities.Challanges.Where(x => x.IsAccepted == true && x.IsCompleted != true && x.UserId == model.UserId).OrderByDescending(x=>x.StartDateTime).ToList();
            List<ChallengeStartSoonModel> challengeStartSoons = new List<ChallengeStartSoonModel>();
            foreach(var item in data)
            {
                if (item.ChallangeStartDateTime > DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00) && item.ChallangeStartDateTime != null)
                {

                    var ChallengeData = new ChallengeStartSoonModel();
                    var AdminName = entities.Challanges.Where(x => x.ChallangeId == item.ChallangeId).Select(x => x.Name).FirstOrDefault();
                    ChallengeData.ChallengeId = (int)item.ChallangeId;
                    ChallengeData.UserId = item.UserId;
                    ChallengeData.CreatedByUsername = AdminName;
                    ChallengeData.challengeCreatedDatetime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", item.StartDateTime);
                    ChallengeData.ChallengeStartDateTime = string.Format("{0:dd MMMM, yyyy hh:mm tt}", item.ChallangeStartDateTime);
                    challengeStartSoons.Add(ChallengeData);
                }
            }
            return challengeStartSoons;
        }
        #endregion

        #region Set Starting Challenge Time
        public string SetStartingChallengeTime(ChallangeIdModel model)
        {
            var ChallengeStartDateTime = DateTime.UtcNow.AddHours(5.00).AddMinutes(32.00);
            var data = entities.Database.ExecuteSqlCommand("Update Challange  set ChallangeStartDateTime='" + ChallengeStartDateTime + "'where ChallangeId=" + model.ChallangeId);
            if (data > 0)
            {
                return string.Format("{0:dd MMMM, yyyy hh:mm tt}", ChallengeStartDateTime);
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}