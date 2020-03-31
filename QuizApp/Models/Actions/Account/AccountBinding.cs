﻿using Newtonsoft.Json;
using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;


namespace QuizApp.Models
{
    public class AccountBinding
    {
        #region Database Entities Declaration
        QuizAppEntities entities = new QuizAppEntities();
        #endregion

        #region GetRefferal Code
        public TokenResult GetRefferlCode(string userId)
        {
            return entities.Users.Where(x => x.UserID == userId).Select(x => new TokenResult() {
                RefferalCode = x.ReferalCode,
                UserName = x.Name
            }).FirstOrDefault();

        }
        #endregion

        #region Register
        public TokenResult RegisterUser(RegisterBindingModel model)
        {
            try
            {
                var Otp = GeneralFunctions.GetOTP();
                //Set By Defaulte Admin Refferal Code When User Not Use Any Refferal Code
                if (model.UsedReferalCode == null)
                {
                    string AdminId = ConfigurationManager.AppSettings["Admin"].ToString(); ;
                    model.UsedReferalCode = entities.Users.Where(x => x.UserID == AdminId).Select(x => x.ReferalCode).FirstOrDefault();
                }

                var ParentIDs = GetParentsIDsFromReferalCode(model.UsedReferalCode, model.UserId);

                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    User registerUser = new User()
                    {
                        Name = model.Name,
                        Password = model.Password,
                        UserID = model.UserId,
                        CreatedDate = DateTime.Now,
                        LastUpdateDate = DateTime.Now,
                        ReferalCode = GeneralFunctions.GetReferalCode(),
                        DeviceID = model.DeviceID,
                        LastActiveDate = DateTime.Now,
                        IP = model.IP,
                        isActive = true,
                        isBlocked = false,
                        NotificationKey = model.NotificationKey,
                        otp = Otp.ToString(),
                        ParentIDs = ParentIDs,
                        Platform = model.Platform,
                        UsedReferalCode = model.UsedReferalCode,
                    };
                    var rrea = sms_api_callAsync(model.PhoneNumber, Otp.ToString());
                    MobileOTP mobileOTP = new MobileOTP()
                    {
                        PhoneNumber = model.PhoneNumber,
                        OTP = Otp,
                        CreatedDate = DateTime.Now
                    };

                    entities.MobileOTPs.Add(mobileOTP);
                    entities.Users.Add(registerUser);
                    int updatedRow = entities.SaveChanges();
                    var UserId = registerUser.UserID;

                    if (updatedRow >= 1)
                    {
                        //Add Registration Income
                        var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
                        EaningHeadModel earningHeads = new EaningHeadModel();
                        using (StreamReader r = new StreamReader(jsonFilePath))
                        {
                            string json = r.ReadToEnd();
                            earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
                        }
                        var data = (from U in entities.Users
                                    join A in entities.AspNetUsers on U.UserID equals A.Id where U.UserID == UserId
                                    select new UserTransactionModel() {
                                        UserName = U.Name,
                                        MobileNumber = A.UserName
                                    }).FirstOrDefault();
                        var uniqueKey = $"{UserId}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                        Transaction transaction = new Transaction()
                        {
                            UserID = UserId,
                            transactionDateTime = DateTime.Now,
                            UniqueKey = uniqueKey,
                            paymentStatus = "Earning",
                            amount = earningHeads.RegistrationIncome,
                            comment = "Registration Income Amount",
                            username = data.UserName,
                            mobilenumber = data.MobileNumber
                        };
                        entities.Transactions.Add(transaction);
                        entities.SaveChanges();

                        AuthRepository authRepository = new AuthRepository();
                        return authRepository.GenerateToken(model.PhoneNumber, model.Password, model.UserId, "");
                    }
                    else
                    {
                        TokenResult result = new TokenResult();
                        result.result = false;
                        result.error_message = "User registration failed.";
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                TokenResult result = new TokenResult();
                result.result = false;
                result.error_message = ex.Message;
                return result;
            }
        }
        #endregion

        #region Pass Code Save
        public bool PassCodeSave(RegisterBindingModel model)
        {
            var data = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if (data != null)
            {
                data.Passcode = model.Passcode;
                entities.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Referal Code
        public bool ValidateReferalCode(string usedReferalCode, string Username)
        {
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var user = entities.Users.Where(x => x.ReferalCode == usedReferalCode && x.AspNetUser.UserName != Username).FirstOrDefault();

                return user == null;
            }
        }

        public string GetParentsIDsFromReferalCode(string usedReferalCode, string userId)
        {
            string RefCode = "";
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var user = entities.Users.Where(x => x.ReferalCode == usedReferalCode && x.UserID != userId).FirstOrDefault();
                if (user != null)
                {
                    if (!string.IsNullOrEmpty(user.ParentIDs))
                    {
                        RefCode = user.UserID + "," + user.ParentIDs;
                        RefCode = UpdateParentIDsFromReferalCode(RefCode);
                    }
                    else
                    {
                        RefCode = user.UserID;
                    }
                }

            }
            return RefCode;
        }
        public string UpdateParentIDsFromReferalCode(string RefCode)
        {
            string[] Ref = RefCode.Split(',');
            if (Ref.Length >= 11)
            {
                for (int i = Ref.Length - 1, j = Ref.Length - 2; i >= 0; i--, j--)
                {

                    if (i == 0)
                    {
                        Ref[i] = "";
                    }
                    else
                    {
                        Ref[i] = Ref[j];
                    }

                }
                Ref = Ref.Where(w => w != Ref[0]).ToArray();
                return RefCode = string.Join(",", Ref);
            }
            else
            {
                return RefCode;
            }
        }
        #endregion

        #region OTP
        #region OTP Verification at Register Time
        public bool OTPVerification(OTPVerificationBindingModel model)
        {
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var userInfo = entities.MobileOTPs.Where(x => x.PhoneNumber == model.PhoneNumber).OrderByDescending(a => a.CreatedDate).FirstOrDefault();
                if (userInfo != null && userInfo.OTP == model.OTP)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Add OTP ForgotPassword
        public bool AddOTP(ForgotPasswordBindingModel model)
        {
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                MobileOTP mobileOTP = new MobileOTP()
                {
                    PhoneNumber = model.PhoneNumber,
                    OTP = GeneralFunctions.GetOTP(),
                    CreatedDate = DateTime.Now,

                };

                entities.MobileOTPs.Add(mobileOTP);
                return entities.SaveChanges() > 0;
            }
        }
        #endregion
        #endregion

        #region Password
        public bool UpdatePassword(string UserId, string Password)
        {
            using (QuizAppEntities model = new QuizAppEntities())
            {
                var userInfo = model.Users.Where(x => x.UserID == UserId).FirstOrDefault();

                if (userInfo != null)
                {
                    userInfo.Password = Password;
                    userInfo.LastUpdateDate = DateTime.Now;
                    model.Entry(userInfo).State = EntityState.Modified;
                    return model.SaveChanges() > 0;

                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        #region Add Bank Account Details
        public bool AddBankAccountDetails(AddBankAccountDetailsModel model)
        {
            var data = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if (data != null)
            {
                data.AccountNumber = model.AccountNumber;
                data.NameInAccount = model.NameInAccount;
                data.Bank = model.Bank;
                data.IFSCCode = model.IFSCCode;
                return entities.SaveChanges() > 0;
            }
            else
            {
                return false;
            }
        }
        #endregion

        #region Current Amount Details
        public CurrentAmountDetailsModel CurrentAmountDetails(string UserId)
        {
            var data = entities.Users.Where(x => x.UserID == UserId).FirstOrDefault();
            GeneralFunctions generalFunctions = new GeneralFunctions();
            var earning = generalFunctions.getEarningHeads();
            return new CurrentAmountDetailsModel()
            {
                AccountNumber = data.AccountNumber != null ? data.AccountNumber : "",
                NameInAccount = data.NameInAccount != null ? data.NameInAccount : "",
                IFSCCode = data.IFSCCode != null ? data.IFSCCode : "",
                Bank = data.Bank != null ? data.Bank : "",
                amount = data.CurrentBalance != null ? Convert.ToDouble(generalFunctions.GetDecimalvalue(data.CurrentBalance.ToString())) : 0,
                chargesAmount = earning.WithdrawCharges
            };
        }
        #endregion

        #region Withdrawal Amount
        public WithdrawalAmountBalance WithdrawalAmount(WithdrawalAmountModel model)
        {

            var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
            EaningHeadModel earningHeads = new EaningHeadModel();
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
            }

            WithdrawalAmountBalance withdrawal = new WithdrawalAmountBalance();
            string passcode = entities.Users.Where(x => x.UserID == model.UserId && x.Passcode == model.Passcode).Select(x => x.Passcode).FirstOrDefault();
            if (passcode != null)
            {
                var data = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
                var data1 = entities.AspNetUsers.Where(x => x.Id == model.UserId).FirstOrDefault();
                if (data.CurrentBalance >= model.amount)
                {
                    if (earningHeads.MaximumWithdrawLimit >= model.amount && model.amount >= earningHeads.MinimumWithdrawlLimit)
                    {
                        var uniqueKey = $"{data.UserID}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                        if (model.WithdrawType == "Bank")
                        {
                            //var WithdrawalAmount = model.amount - earningHeads.WithdrawCharges;
                            //var WithdrawalCharges = earningHeads.WithdrawCharges;

                            if (model.AccountNumber != null && model.NameInAccount != null && model.IFSCCode != null && model.Bank != null && model.amount.ToString() != null)
                            {
                                //Entery in Transaction Table for Withdrawal Amount in Bank
                                Transaction transaction = new Transaction()
                                {
                                    UserID = model.UserId,
                                    transactionDateTime = DateTime.Now,
                                    UniqueKey = uniqueKey,
                                    paymentStatus = "Pending",
                                    amount = model.amount,
                                    comment = "Withdrawal Amount in Bank",
                                    username = data.Name,
                                    mobilenumber = data1.UserName,
                                    WithdrawType = model.WithdrawType,
                                    AccountNumber = model.AccountNumber,
                                    NameInAccount = model.NameInAccount,
                                    Bank = model.Bank,
                                    IFSCCode = model.IFSCCode
                                };
                                ////Entery in Transaction Table for Bank Charges
                                //Transaction charges = new Transaction()
                                //{
                                //    UserID = model.UserId,
                                //    transactionDateTime = DateTime.Now,
                                //    UniqueKey = uniqueKey,
                                //    paymentStatus = "Pending",
                                //    amount = WithdrawalCharges,
                                //    comment = "Withdrawal Amount Bank Charges",
                                //    username = data.Name,
                                //    mobilenumber = data1.UserName,
                                //    WithdrawType = model.WithdrawType,
                                //    AccountNumber = model.AccountNumber,
                                //    NameInAccount = model.NameInAccount,
                                //    Bank = model.Bank,
                                //    IFSCCode = model.IFSCCode
                                //};
                                entities.Transactions.Add(transaction);
                                //entities.Transactions.Add(charges);
                                entities.SaveChanges();
                                withdrawal = new WithdrawalAmountBalance()
                                {
                                    State = "True",
                                    Balance = model.amount,
                                    status = "Pending"
                                };
                            }
                            else
                            {
                                withdrawal = new WithdrawalAmountBalance()
                                {
                                    State = "model",
                                    Balance = model.amount
                                };
                            }
                        }
                        else if (model.WithdrawType == "Paytm")
                        {
                            string hostName = Dns.GetHostName();// Retrive the Name of HOST  
                                                                // Get the IP  
                            string myIP = Dns.GetHostByName(hostName).AddressList[0].ToString();

                            //var WithdrawalAmount = model.amount - earningHeads.WithdrawCharges;
                            //var WithdrawalCharges = earningHeads.WithdrawCharges;


                            //string pay = "Pending";
                            PaytmBinding paytmBinding = new PaytmBinding();
                            var pay = paytmBinding.PaytmResponse(data1.UserName, "Withdrawal Amount in Paytm", Convert.ToString(model.amount), myIP);
                            string paymentStatus = string.Empty;
                            paytmResponse paytmResponse = new paytmResponse();
                            var paytmResult = JsonConvert.DeserializeObject<paytmResponse.Root>(pay);
                            if (paytmResult.statusCode == "SUCCESS" && paytmResult.status == "SUCCESS")
                            {
                                paymentStatus = "withdrawal";
                            }
                            else
                            {
                                paymentStatus = "pending";
                            }
                                Transaction transaction = new Transaction()
                                {
                                    UserID = model.UserId,
                                    transactionDateTime = DateTime.Now,
                                    UniqueKey = uniqueKey,
                                    paymentStatus = paymentStatus,
                                    amount = model.amount,
                                    comment = paytmResult.metadata,
                                    username = data.Name,
                                    mobilenumber = data1.UserName,
                                    PaytmWithdrawCharges = 0,//earningHeads.WithdrawCharges,
                                    PaytmOrderId = paytmResult.orderId,
                                    PaytmResponse = pay
                                };
                                //Transaction charges = new Transaction()
                                //{
                                //    UserID = model.UserId,
                                //    transactionDateTime = DateTime.Now,
                                //    UniqueKey = uniqueKey,
                                //    paymentStatus = pay,
                                //    amount = WithdrawalCharges,
                                //    comment = "Withdrawal Amount in Paytm Charges",
                                //    username = data.Name,
                                //    mobilenumber = data1.UserName,
                                //    PaytmWithdrawCharges = earningHeads.WithdrawCharges,
                                //    PaytmOrderId = orderId,
                                //    PaytmResponse = pay
                                //};
                                entities.Transactions.Add(transaction);
                                //entities.Transactions.Add(charges);
                                entities.SaveChanges();

                                withdrawal = new WithdrawalAmountBalance()
                                {
                                    State = "True",
                                    Balance = model.amount,
                                    status = paytmResult.status
                                };
                            
                        }
                    }
                    else
                    {
                        withdrawal = new WithdrawalAmountBalance()
                        {
                            State = "insufficient",
                            Balance = model.amount
                        };
                    }
                }
                else
                {
                    withdrawal = new WithdrawalAmountBalance()
                    {
                        State = "CurrentBalance",
                        Balance = model.amount
                    };
                }
            }

            else
            {
                withdrawal = new WithdrawalAmountBalance()
                {
                    State = "Passcode",
                    Balance = model.amount
                };
            }
            return withdrawal;
        }
        #endregion

        #region Points Redeem
        public RedeemBalanceModel PointRedeem(PointsRedeemModel model)
        {
            string passcode = entities.Users.Where(x => x.Passcode == model.Passcode).Select(x => x.Passcode).FirstOrDefault();
            if (passcode != null)
            {
                var data = entities.Users.Where(x => x.UserID == model.UserID).FirstOrDefault();
                var data1 = entities.AspNetUsers.Where(x => x.Id == model.UserID).FirstOrDefault();
                GeneralFunctions general = new GeneralFunctions();
                if (data.CurrentPoint >= model.PointsWithdraw && general.PointReddemValueCheck(model.PointsWithdraw))
                {
                    EaningHeadModel earningHeads = new EaningHeadModel();
                    var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
                    using (StreamReader r = new StreamReader(jsonFilePath))
                    {
                        string json = r.ReadToEnd();
                        earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
                    }
                    //Insert User Point Table
                    UserPoint Point = new UserPoint()
                    {
                        UserID = model.UserID,
                        TransactionDate = DateTime.Now,
                        PointsWithdraw = model.PointsWithdraw,
                        PointsEarned = 0,
                        Description = "Point Withdrawal in Account",
                        CreatedDate = DateTime.Now
                    };
                    entities.UserPoints.Add(Point);
                    entities.SaveChanges();

                    //Insert Transaction Table
                    double balance = model.PointsWithdraw * earningHeads.PointAmount;
                    GeneralFunctions generalFunctions = new GeneralFunctions();
                    balance = Convert.ToDouble(generalFunctions.GetDecimalvalue(balance.ToString()));
                    var uniqueKey = $"{data.UserID}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                    Transaction transaction = new Transaction()
                    {
                        UserID = model.UserID,
                        transactionDateTime = DateTime.Now,
                        UniqueKey = uniqueKey,
                        paymentStatus = "points",
                        amount = balance,
                        comment = "Point Withdrawal in Your Current Balance",
                        username = data.Name,
                        mobilenumber = data1.UserName,
                        ConvertedPoints = model.PointsWithdraw
                    };
                    entities.Transactions.Add(transaction);
                    entities.SaveChanges();
                    return new RedeemBalanceModel()
                    {
                        RedeemBalance = balance,
                        State = "True"
                    };
                }
                else
                {
                    return new RedeemBalanceModel()
                    {
                        RedeemBalance = 0,
                        State = "insufficient"
                    };
                }
            }
            else
            {
                return new RedeemBalanceModel()
                {
                    RedeemBalance = 0,
                    State = "Passcode"
                };
            }
        }
        #endregion

        #region Get User Information isActive or Blocked
        public User GetUserInformation(string UserId)
        {
            return entities.Users.Where(x => x.UserID == UserId).FirstOrDefault();
        }
        #endregion

        #region Get User Profile
        public UserProfileModel GetUserProfile(UserModel model)
        {
            var UserInfo = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
            if (UserInfo != null)
            {
                var data = new UserProfileModel();

                var UserInfoPhoneNumber = entities.AspNetUsers.Where(x => x.Id == model.UserId).Select(x => x.PhoneNumber).FirstOrDefault();
                //User Information
                data.ReferalCode = UserInfo.ReferalCode;
                data.Name = UserInfo.Name;
                data.PhoneNumber = UserInfoPhoneNumber;

                var ParentUserInfo = entities.Users.Where(x => x.ReferalCode == UserInfo.UsedReferalCode).FirstOrDefault();
                if (ParentUserInfo != null)
                {
                    var ParentPhoneNumber = entities.AspNetUsers.Where(x => x.Id == ParentUserInfo.UserID).Select(x => x.PhoneNumber).FirstOrDefault();
                    data.ParentName = ParentUserInfo.Name;
                    data.ParentPhoneNumber = ParentPhoneNumber;
                }
                return data;
                //return new UserProfileModel()
                //{
                //    ReferalCode = UserInfo.ReferalCode,
                //    Name = UserInfo.Name,
                //    PhoneNumber = entities.AspNetUsers.Where(x => x.Id == model.UserId).Select(x => x.PhoneNumber).FirstOrDefault(),
                //    ParentName = ParentUserInfo.Name!=null?ParentUserInfo.Name:string.Empty,
                //    ParentPhoneNumber = entities.AspNetUsers.Where(x => x.Id == ParentUserInfo.UserID).Select(x => x.PhoneNumber).FirstOrDefault(),
                //};
            }
            else
            {
                return null;
            }
        }
        #endregion

        #region Get Sms 
        public async Task<string> sms_api_callAsync(string mobile, string uniqueNumber)
        {
            try
            {
                //$username = "motanad";
                //$password = "yadav@123456";
                string message = uniqueNumber + " " + "%20is%20your%20Quiz%20verification%20code";
                string url = "http://msg.msgclub.net/rest/services/sendSMS/sendGroupSms?AUTH_KEY=c24eb7fe1cecdca943c82c40976ea6c1&message=" + message + "&senderId=MOTANA&routeId=1&mobileNos=" + mobile + "&smsContentType=english";

                HttpClient client = new HttpClient();

                HttpResponseMessage response = await client.GetAsync(url);
                return response.EnsureSuccessStatusCode().ToString();
                //return "Test";
            }
            catch(Exception ex)
            {
                var data = ex.Message;
                return data;
            }
        }
        #endregion
    }
}