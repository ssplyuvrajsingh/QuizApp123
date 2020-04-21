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
                var data = challangeBinding.AddUserChallange(model);
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
    }
}