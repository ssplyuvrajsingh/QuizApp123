using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models.Output
{
    public class AllUser
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Platform { get; set; }
        public string PhoneNumber { get; set; }
        public string UserId { get; set; }
        public string ReferalCode { get; set; }
        public string UsedReferalCode { get; set; }

         public int ID { get; set; }
        public string UserID { get; set; }
        public string ParentIDs { get; set; }
        public string Name { get; set; }
        public string Password { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string DeviceID { get; set; }
        public string Platform { get; set; }
        public Nullable<System.DateTime> LastActiveDate { get; set; }
        public string ReferalCode { get; set; }
        public string UsedReferalCode { get; set; }
        public string NotificationKey { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<bool> isBlocked { get; set; }
        public string otp { get; set; }
        public string IP { get; set; }
        public System.DateTime LastUpdateDate { get; set; }
        public Nullable<int> CurrentPoint { get; set; }
        public Nullable<double> CurrentBalance { get; set; }
        public Nullable<double> TotalEarn { get; set; }
        public Nullable<double> TotalWithdraw { get; set; }
        public Nullable<double> PendingWithdraw { get; set; }
        public Nullable<double> MothlyIncome { get; set; }
        public string AccountNumber { get; set; }
        public string NameInAccount { get; set; }
        public string Bank { get; set; }
        public string IFSCCode { get; set; }
        public string Passcode { get; set; }
    }
}