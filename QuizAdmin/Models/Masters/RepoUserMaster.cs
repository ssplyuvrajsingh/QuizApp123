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
        #region User list
        public List<AllUser> getUser()
        {
            var allusers = (from u in db.Users
                            join au in db.AspNetUsers on u.UserID equals au.Id
                            where u.UserID == au.Id
                            select new AllUser
                            {
                                Name = u.Name,
                                PhoneNumber = au.PhoneNumber,
                                ReferalCode = u.ReferalCode,
                                UsedReferalCode = u.UsedReferalCode,
                                Password = u.Password,
                                CreatedDate=u.CreatedDate,
                                LastActiveDate = u.LastActiveDate,
                                isActive = u.isActive,
                                isBlocked = u.isBlocked,
                                
                                CurrentPoint = u.CurrentPoint,
                                CurrentBalance =u.CurrentBalance,
                                TotalEarn = u.TotalEarn,
                                TotalWithdraw = u.TotalWithdraw,
                                PendingWithdraw = u.PendingWithdraw,
                                MothlyIncome = u.MothlyIncome,
                                AccountNumber = u.AccountNumber,
                                NameInAccount = u.NameInAccount,
                                Bank = u.Bank,
                                IFSCCode = u.IFSCCode,
                                Passcode = u.Passcode,
                                UserId = u.UserID,
                                ParentIDs = u.ParentIDs,

                            }).ToList();
            return allusers;
        }
        #endregion

        #region Transaction List

        public List<TransactionModel> GetTransactions()
        {
            return db.Transactions.Select(s => new TransactionModel()
            {
                TransactionID = s.TransactionID,
                transactionDateTime = s.transactionDateTime.Value,
                paymentStatus = s.paymentStatus,
                amount = s.amount.Value,
                comment = s.comment,
                username = s.username,
                mobilenumber = s.mobilenumber,
                ConvertedPoints=s.ConvertedPoints,
                WithdrawType=s.WithdrawType,
                AccountNumber=s.AccountNumber,
                NameInAccount=s.NameInAccount,
                Bank=s.Bank,
                IFSCCode=s.IFSCCode,
                PaytmOrderId=s.PaytmOrderId,
                PaytmWithdrawCharges=s.PaytmWithdrawCharges,
                PaytmResponse=s.PaytmResponse
            }).Where(x=>x.paymentStatus!="Pending").OrderByDescending(x=>x.transactionDateTime).ToList();
        }
        #endregion

        #region Transactions Pending List
        public List<TransactionModel> GetTransactionsPending()
        {
            return db.Transactions.Select(s => new TransactionModel()
            {
                TransactionID = s.TransactionID,
                transactionDateTime = s.transactionDateTime.Value,
                paymentStatus = s.paymentStatus,
                amount = s.amount.Value,
                comment = s.comment,
                username = s.username,
                mobilenumber = s.mobilenumber,
                ConvertedPoints = s.ConvertedPoints,
                WithdrawType = s.WithdrawType,
                AccountNumber = s.AccountNumber,
                NameInAccount = s.NameInAccount,
                Bank = s.Bank,
                IFSCCode = s.IFSCCode,
                PaytmOrderId = s.PaytmOrderId,
                PaytmWithdrawCharges = s.PaytmWithdrawCharges,
                PaytmResponse = s.PaytmResponse
            }).Where(x => x.paymentStatus == "Pending").OrderByDescending(x => x.transactionDateTime).ToList();
        }
        #endregion
    }
}