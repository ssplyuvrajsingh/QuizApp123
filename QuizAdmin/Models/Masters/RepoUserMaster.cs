using Microsoft.AspNet.Identity;
using QuizAdmin.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuizAdmin.Models
{
    public class RepoUserMaster
    {
        QuizAppEntities db = new QuizAppEntities();
        #region Quiz list
        public List<AllUser> getUser()
        {
            var allusers = (from u in db.Users
                            join au in db.AspNetUsers on u.UserID equals au.Id
                            where u.UserID == au.Id
                            select new AllUser
                            {
                                Name = u.Name,
                                ParentIDs = u.ParentIDs,
                                Passcode=u.Passcode,
                                Password=u.Password,
                                LastActiveDate=u.LastActiveDate,
                                ReferalCode = u.ReferalCode,
                                UsedReferalCode = u.UsedReferalCode,
                                isActive=u.isActive,
                                isBlocked=u.isBlocked,
                                CurrentPoint=u.CurrentPoint,
                                TotalEarn=u.TotalEarn,
                                TotalWithdraw=u.TotalWithdraw,
                                PendingWithdraw=u.PendingWithdraw,
                                MothlyIncome=u.MothlyIncome,
                                AccountNumber=u.AccountNumber,
                                NameInAccount=u.NameInAccount,
                                Bank=u.Bank,
                                IFSCCode=u.IFSCCode,
                               
                                PhoneNumber = u.,
                                Platform = u.Platform,
                                UserId = u.UserID,
                                
                                
                                
                            }).ToList();
            return allusers;
        }
        #endregion
    }
}