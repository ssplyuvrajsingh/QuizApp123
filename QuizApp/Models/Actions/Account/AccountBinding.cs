using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                if (!string.IsNullOrEmpty(user.ParentIDs))
                {
                    if (user != null)
                    {
                        RefCode = user.ParentIDs + "," + user.UserID;
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
                for (int i = 0, j = 1; i < Ref.Length; i++, j++)
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
    }
}