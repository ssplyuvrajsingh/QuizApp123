using Newtonsoft.Json;
using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuizApp.Models
{
    public class AccountBinding
    {
        #region Database Entities Declaration
        QuizAppEntities entities = new QuizAppEntities();
        #endregion

        #region Register
        public TokenResult RegisterUser(RegisterBindingModel model)
        {
            try
            {
                var Otp = GeneralFunctions.GetOTP();
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
                        isActive = false,
                        isBlocked = false,
                        NotificationKey = model.NotificationKey,
                        otp = Otp.ToString(),
                        ParentIDs = ParentIDs,
                        Platform = model.Platform,
                        UsedReferalCode = model.UsedReferalCode
                    };

                    MobileOTP mobileOTP = new MobileOTP()
                    {
                        PhoneNumber = model.PhoneNumber,
                        OTP = Otp,
                        CreatedDate = DateTime.Now
                    };

                    entities.MobileOTPs.Add(mobileOTP);
                    entities.Users.Add(registerUser);
                    int updatedRow = entities.SaveChanges();

                    if (updatedRow >= 1)
                    {
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
            if (Ref.Length >= 10)
            {
                for (int i = Ref.Length, j = 1; i > Ref.Length; i--, j--)
                {
                    if (i == Ref.Length - 1)
                    {
                        Ref[i] = "";
                    }
                    else
                    {
                        Ref[i] = Ref[j];
                    }

                }
                Ref = Ref.Where(w => w != Ref[Ref.Length - 1]).ToArray();
                return RefCode = string.Join(",", Ref);
            }
            else
            {
                return RefCode;
            }
        }
        #endregion

        #region OTP
        public bool OTPVerification(OTPVerificationBindingModel model)
        {
            var result = false;
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var userInfo = entities.MobileOTPs.Where(x => x.PhoneNumber == model.PhoneNumber).OrderByDescending(a => a.CreatedDate).FirstOrDefault();
                if (userInfo != null && userInfo.OTP == model.OTP)
                {
                    return true;
                }
                return result;
            }
        }

        public bool AddOTP(ForgotPasswordBindingModel model)
        {
            using (QuizAppEntities entities = new QuizAppEntities())
            {
                MobileOTP mobileOTP = new MobileOTP()
                {
                    PhoneNumber = model.PhoneNumber,
                    OTP = GeneralFunctions.GetOTP(),
                    CreatedDate = DateTime.Now
                };

                entities.MobileOTPs.Add(mobileOTP);
                return entities.SaveChanges() > 0;
            }
        }
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
            return new CurrentAmountDetailsModel()
            {
                AccountNumber = data.AccountNumber != null ? data.AccountNumber : "",
                NameInAccount = data.NameInAccount != null ? data.NameInAccount : "",
                IFSCCode = data.IFSCCode != null ? data.IFSCCode : "",
                Bank =data.Bank != null ? data.Bank : "",
                amount = data.CurrentBalance != null ? (double)data.CurrentBalance : 0
            };
        }
        #endregion

        #region Withdrawal Amount
        public string WithdrawalAmount(WithdrawalAmountModel model)
        {
            string passcode = entities.Users.Where(x => x.Passcode == model.Passcode).Select(x => x.Passcode).FirstOrDefault();
            if (passcode != null)
            {
                var data = entities.Users.Where(x => x.UserID == model.UserId).FirstOrDefault();
                var data1 = entities.AspNetUsers.Where(x => x.Id == model.UserId).FirstOrDefault();
                if (data.CurrentBalance >= model.amount)
                {
                    var uniqueKey = $"{data.UserID}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                    Transaction transaction = new Transaction()
                    {
                        UserID = model.UserId,
                        transactionDateTime = DateTime.Now,
                        UniqueKey = uniqueKey,
                        paymentStatus = "Withdraw",
                        amount = model.amount,
                        comment = "",
                        username = data.Name,
                        mobilenumber = data1.UserName,
                        WithdrawType = model.WithdrawType,
                        AccountNumber = model.AccountNumber,
                        NameInAccount = model.NameInAccount,
                        Bank = model.Bank,
                        IFSCCode = model.IFSCCode
                    };
                    entities.Transactions.Add(transaction);
                    entities.SaveChanges();
                    return "True";
                }
                else
                {
                    return "insufficient";
                }
            }
            else
            {
                return "Passcode";
            }
        }
        #endregion

        #region Points Redeem
        public string PointRedeem(PointsRedeemModel model)
        {

            var data = entities.Users.Where(x => x.UserID == model.UserID).FirstOrDefault();
            var data1 = entities.AspNetUsers.Where(x => x.Id == model.UserID).FirstOrDefault();
            if (data.CurrentPoint >= model.PointsWithdraw)
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
                var uniqueKey = $"{data.UserID}~{DateTime.Now.ToString("dd-MM-yyy")}~Earning";
                Transaction transaction = new Transaction()
                {
                    UserID = model.UserID,
                    transactionDateTime = DateTime.Now,
                    UniqueKey = uniqueKey,
                    paymentStatus = "points",
                    amount = balance,
                    comment = "Point Withdrawal in Account",
                    username = data.Name,
                    mobilenumber = data1.UserName,
                };
                entities.Transactions.Add(transaction);
                entities.SaveChanges();
                return "True";
            }
            else
            {
                return "insufficient";
            }
        }
        #endregion
    }
}