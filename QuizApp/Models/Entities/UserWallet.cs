//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace QuizApp.Models.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class UserWallet
    {
        public int WalletID { get; set; }
        public string UserID { get; set; }
        public Nullable<int> CurrentPoint { get; set; }
        public Nullable<int> CurrentBalance { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public Nullable<int> TotalEarn { get; set; }
        public Nullable<int> TotalWithdraw { get; set; }
        public Nullable<int> PendingWithdraw { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
