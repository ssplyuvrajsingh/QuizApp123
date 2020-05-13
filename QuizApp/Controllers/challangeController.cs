using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Http;
using QuizApp.Models;


namespace QuizApp.Controllers
{
    public class challangeController : ApiController
    {
        #region Challange

        #region First Screen Create Challange
        //POST api/challange/ChallangePost
        [HttpPost]
        public ResultClass ChallangePost(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                    ChallangeBinding challangeBinding = new ChallangeBinding();
                    var data = challangeBinding.ChallangePost(model);
                    if (data!=null)
                    {
                        Result = new ResultClass()
                        {
                            Data = data,
                            Message = "Data Insert successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        Result = new ResultClass()
                        {
                            Data = null,
                            Message = "Data not Insert",
                            Result = false
                        };
                    }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Second Screen Search Users
        [HttpPost]
        public ResultClass SearchUsers(SearchKeyModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SearcUser(model);
                if (data.Count() > 0)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Data = null,
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Third Screen Add User for Challange
        [HttpPost]
        public ResultClass AddUserChallange(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                bool data = challangeBinding.AddUserChallange(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "User added successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "User not added",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Added User List
        [HttpPost]
        public ResultClass AddUserList(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.AddedUserList(model);
                if (data.AddUsers.Any())
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Added user found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Added user not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Challange Accept
        [HttpPost]
        public ResultClass ChallangeAccept(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.ChallangeAccept(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = null,
                        Message = "Challange accepted successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Challange not accepted",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Fourth Screen Add Invest Point for Challange  
        [HttpPost]
        public ResultClass AddInvestPoint(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                bool data = challangeBinding.AddInvestPoint(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Message = "Data save successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Your points are insufficient",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Five Screen Get Challangers List
        [HttpPost]
        public ResultClass GetChallangersList(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.GetChallangersList(model);
                if (data != null)
                {
                    Result = new ResultClass()
                    {
                        Data=data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Set Challange Result
        [HttpPost]
        public ResultClass SetChallangeResult(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SetChallangeResult(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data save successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not save",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #endregion

        #region Rewards Request for Points
        [HttpPost]
        public ResultClass RewardsPoints(UserModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.RewardsPoints(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Points rewards successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Points not rewards",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Winner Users List
        [HttpPost]
        public ResultClass WinnerUsersList()
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.WinnerUsersList();
                if (data != null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Winner User Details
        [HttpPost]
        public ResultClass WinnerUserDetails(WinnerUsersModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.WinnerUserDetails(model);
                if (data != null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region SavedChallangeList
        [HttpPost]
        public ResultClass SaveChallangeList(UserModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SaveChallangeList(model);
                if (data.Any())
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Request Challange List
        [HttpPost]
        public ResultClass RequestChallangeList(UserModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.RequestChallangeList(model);
                if (data.Any())
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Get Challanges Id's according user
        [HttpPost]
        public ResultClass GetChallangesIds(UserModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.GetNotificationForChallange(model);
                if (data.Any())
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Delete saved challenge
        [HttpPost]
        public ResultClass DeleteSavedChallenge(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.DeleteSavedChallenge(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data deleted successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not deleted",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Get Challenge Time
        [HttpPost]
        public ResultClass GetChallengeTime(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.GetChallengeTime(model);
                if (data!=null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Data=data,
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Set Temporary Winner
        [HttpPost]
        public ResultClass SetTemporaryWinner(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SetTemporaryWinner(model);
                if (data!=null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data save successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not deleted",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region SetWinnerUser
        [HttpPost]
        public ResultClass SetWinnerUser(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SetWinnerUser(model);
                if (data!=null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data deleted successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not deleted",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Save Minimum Entry Points
        [HttpPost]
        public ResultClass SaveMinimumEntryPoints(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SaveMinimumEntryPoints(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Points save successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Your points are insufficient",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Send Challenge Start Notification
        [HttpPost]
        public ResultClass SendChallengeStartNotification(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SendChallengeStartNotification(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = null,
                        Message = "Message send successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Message not send",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Challenge Starting Soon
        [HttpPost]
        public ResultClass ChallengeStartingSoon(UserModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.ChallengeStartingSoon(model);
                if (data.Any())
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Data not fond",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Set Starting Challenge Time
        [HttpPost]
        public ResultClass SetStartingChallengeTime(ChallangeIdModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.SetStartingChallengeTime(model);
                if (data!=null)
                {
                    Result = new ResultClass()
                    {
                        Data = data,
                        Message = "Challenge starting time set successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Challenge Starting Time not set",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion

        #region Reject Challenge Request
        [HttpPost]
        public ResultClass RejectChallengeRequest(ChallangeModel model)
        {
            var Result = new ResultClass();
            try
            {
                ChallangeBinding challangeBinding = new ChallangeBinding();
                var data = challangeBinding.ChallengeReject(model);
                if (data)
                {
                    Result = new ResultClass()
                    {
                        Data = null,
                        Message = "Challenge rejected successfully",
                        Result = true
                    };
                }
                else
                {
                    Result = new ResultClass()
                    {
                        Message = "Challene not rejected",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                Result = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
            return Result;
        }
        #endregion
    }
}