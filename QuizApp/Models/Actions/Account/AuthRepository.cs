using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

namespace QuizApp.Models
{
    public class AuthRepository : IDisposable
    {
        QuizAppEntities _ctx;

        public AuthRepository()
        {
            _ctx = new QuizAppEntities();
        }

        #region Generate Token
        public TokenResult GenerateToken(string PhoneNumber, string Password, string UserId, string RefreshTokenStr)
        {
            JavaScriptSerializer jss = new JavaScriptSerializer();
            TokenResult tokenResult = new TokenResult();
            tokenResult = jss.Deserialize<TokenResult>(GeneralFunctions.GETDataNew(Models.Constants.Token, "grant_type=password&username=" + PhoneNumber + "&password=" + Password));
            var RefreshToken = new RefreshToken();
            using (AuthRepository _repo = new AuthRepository())
            {
                if (string.IsNullOrEmpty(RefreshTokenStr))
                {
                    RefreshToken = _repo.AddRefreshToken(UserId, tokenResult.access_token);
                    tokenResult.refresh_token = RefreshToken.ProtectedTicket;
                }
                else
                {
                    RefreshToken refreshToken = new RefreshToken();
                    refreshToken = _ctx.RefreshTokens.Where(x => x.UserId == UserId && x.ProtectedTicket == RefreshTokenStr).FirstOrDefault();
                    if (refreshToken != null)
                    {
                        refreshToken.ExpiresUtc = DateTime.Now.AddMinutes(Constants.TimeOfExpireRefreshTokenHours);
                        refreshToken.IssuedUtc = DateTime.Now;
                        _ctx.SaveChanges();
                    }
                    tokenResult.refresh_token = RefreshTokenStr;
                }

                tokenResult.id = UserId;
            }
            tokenResult.result = true;
            return tokenResult;
        }
        #endregion

        #region Refresh Token
        public RefreshToken AddRefreshToken(string UserId, string AccessToken)
        {
            var existingToken = _ctx.RefreshTokens.Where(r => r.UserId == UserId).FirstOrDefault();

            if (existingToken != null)
            {
                var result = RemoveRefreshToken(existingToken);
            }

            RefreshToken refreshToken = new RefreshToken();
            refreshToken.UserId = UserId;
            TimeSpan m = TimeSpan.FromMinutes(1);
            refreshToken.ExpiresUtc =DateTime.Now.AddMinutes(Constants.TimeOfExpireRefreshTokenHours);
            refreshToken.IssuedUtc = DateTime.Now;
            refreshToken.ProtectedTicket = GeneralFunctions.RandomString(100);
            _ctx.RefreshTokens.Add(refreshToken);
            _ctx.SaveChanges();
            return refreshToken;
        }

        public bool RemoveRefreshToken(RefreshToken refreshToken)
        {
            _ctx.RefreshTokens.Remove(refreshToken);
            return _ctx.SaveChanges() > 0;
        }
        #endregion

        #region User
        public User FindUserByRefreshToken(string RefreshToken)
        {
            var refresh = _ctx.RefreshTokens.Where(a => a.ProtectedTicket == RefreshToken).FirstOrDefault();
            if (refresh == null)
            {
                return null;
            }
            else
            {
                if (DateTime.Compare(refresh.ExpiresUtc.Value, DateTime.Now) > 0)
                {
                   var data= _ctx.Users.Where(a => a.UserID == refresh.UserId).FirstOrDefault();
                    return data;
                }
                else
                {
                    return null;
                }
            }
        }
        #endregion

        #region
        public string GetUserName(string UserId)
        {
            var data = _ctx.AspNetUsers.Where(x => x.Id == UserId).Select(x => x.UserName).FirstOrDefault();
            return data;
        }
        #endregion

        public void Dispose()
        {
            _ctx.Dispose();
        }
    }
}