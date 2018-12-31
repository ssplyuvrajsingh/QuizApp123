using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuizApp.Models.Actions
{
    public class AccountBinding
    {
        public ResultClass RegisterUser(RegisterBindingModel model)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    User registerUser = new User()
                    {
                        Name = model.Name,
                        Password = model.Password,
                        UserID = model.UserId,
                        CreatedDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,
                        ReferalCode = model.ReferalCode,
                        DeviceID = model.DeviceID,
                        LastActiveDate = DateTime.Now,
                        IP = model.IP,
                        isActive = model.isActive,
                        isBlocked = model.isBlocked,
                        NotificationKey = model.NotificationKey,
                        otp = model.OTP,
                        ParentIDs = model.ParentIDs,
                        Platform = model.Platform,
                        UsedReferalCode = model.UsedReferalCode
                    };

                    MobileOTP mobileOTP = new MobileOTP()
                    {
                        PhoneNumber = model.PhoneNumber,
                        OTP = int.Parse(model.OTP),
                        CreatedDate = DateTime.Now
                    };

                    entities.MobileOTPs.Add(mobileOTP);
                    entities.Users.Add(registerUser);
                    int updatedRow = entities.SaveChanges();

                    ResultClass result = new ResultClass();

                    if (updatedRow >= 1)
                    {
                        result.Result = true;
                        result.Message = "User register successfully";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "User register fail";
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultClass OTPVerification(RegisterBindingModel model)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var userInfo = entities.MobileOTPs.Where(x => x.PhoneNumber == model.PhoneNumber).ToList();

                    ResultClass result = new ResultClass();

                    if (model.OTP == userInfo[userInfo.Count() - 1].OTP.ToString())
                    {
                        result.Result = true;
                        result.Message = "Correct OTP";
                    }
                    else
                    {
                        result.Result = true;
                        result.Message = "Incorrect OTP";
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultClass AddOTP(ForgotPasswordBindingModel model)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    MobileOTP mobileOTP = new MobileOTP()
                    {
                        PhoneNumber = model.PhoneNumber,
                        OTP = int.Parse(model.OTP),
                        CreatedDate = DateTime.Now
                    };

                    entities.MobileOTPs.Add(mobileOTP);
                    int updatedRow = entities.SaveChanges();

                    ResultClass result = new ResultClass();

                    if (updatedRow >= 1)
                    {
                        result.Result = true;
                        result.Message = "OTP add successfully";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "OTP add fail";
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public async Task<ResultClass> GetAccessToken(LoginBindingModel model)
        {
            try
            {
                var request = HttpContext.Current.Request;
                var tokenServiceUrl = request.Url.GetLeftPart(UriPartial.Authority) + request.ApplicationPath + "token";
                using (var client = new HttpClient())
                {
                    var requestParams = new List<KeyValuePair<string, string>>
                    {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", model.PhoneNumber),
                        new KeyValuePair<string, string>("password", model.Password)
                    };
                    var requestParamsFormUrlEncoded = new FormUrlEncodedContent(requestParams);
                    var tokenServiceResponse = await client.PostAsync(tokenServiceUrl, requestParamsFormUrlEncoded);
                    var responseString = await tokenServiceResponse.Content.ReadAsStringAsync();
                    var responseCode = tokenServiceResponse.StatusCode;
                    var responseMsg = new HttpResponseMessage(responseCode)
                    {
                        Content = new StringContent(responseString, Encoding.UTF8, "application/json")
                    };

                    ResultClass result = new ResultClass();
                    result.Result = responseMsg.IsSuccessStatusCode;

                    if (responseMsg.IsSuccessStatusCode)
                    {
                        var loginmessage = tokenServiceResponse.Content.ReadAsAsync<LoginSuccessMessage>();
                        result.Data = loginmessage.Result.access_token;
                        result.Message = "";
                    }
                    else
                    {
                        var loginmessage = tokenServiceResponse.Content.ReadAsAsync<LoginErrorMessage>();
                        result.Message = loginmessage.Result.error_description;
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public GetUserIdPasswordResponse GetUserIdPassword(string phonenumber)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    string userId = entities.AspNetUsers.Where(x => x.UserName == phonenumber).FirstOrDefault().Id;
                    string password = entities.Users.Where(x => x.UserID == userId).FirstOrDefault().Password;

                    GetUserIdPasswordResponse getUserIdPasswordResponse = new GetUserIdPasswordResponse()
                    {
                        UserId = userId,
                        Password = password
                    };

                    return getUserIdPasswordResponse;
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public ResultClass UpdateRegisterUser(RegisterBindingModel model)
        {
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var userInfo = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
                    ResultClass result = new ResultClass();
                    if (userInfo != null)
                    {
                        userInfo.LastUpdateDate = DateTime.Now;
                        userInfo.ParentIDs = model.ParentIDs;
                        userInfo.UsedReferalCode = model.UsedReferalCode;

                        entities.Entry(userInfo).State = EntityState.Modified;
                        int updatedRow = entities.SaveChanges();

                        if (updatedRow >= 1)
                        {
                            result.Result = true;
                            result.Message = "User register successfully";
                        }
                        else
                        {
                            result.Result = false;
                            result.Message = "User register fail";
                        }
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "User does not exist";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultClass UpdatePassword(string UserId, string Password)
        {
            try
            {
                using (QuizAppEntities model = new QuizAppEntities())
                {
                    var userInfo = model.Users.Where(x => x.UserID == UserId).FirstOrDefault();

                    ResultClass result = new ResultClass();

                    if (userInfo != null)
                    {
                        userInfo.Password = Password;
                        userInfo.LastUpdateDate = DateTime.Now;
                        model.Entry(userInfo).State = EntityState.Modified;
                        int updatedRow = model.SaveChanges();

                        if (updatedRow >= 1)
                        {
                            result.Result = true;
                            result.Message = "Password update successfully";
                        }
                        else
                        {
                            result.Result = false;
                            result.Message = "Password update fail";
                        }
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "User does not exist";
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        public ResultClass GetParentsIDsFromReferalCode(string usedReferalCode, string userId)
        {
            try
            {
                ResultClass result = new ResultClass();
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    var user = entities.Users.Where(x => x.ReferalCode == usedReferalCode && x.UserID != userId).FirstOrDefault();

                    if (user != null)
                    {
                        result.Result = true;
                        if (user.ParentIDs != null)
                        {
                            result.Data = user.ParentIDs + "," + user.ID;
                        }
                        else
                        {
                            result.Data = user.ID;
                        }
                        result.Message = "Valid Referal Code";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "Invalid Referal Code";
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}