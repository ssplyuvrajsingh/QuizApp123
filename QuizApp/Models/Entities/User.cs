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
    
    public partial class User
    {
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
    
        public virtual AspNetUser AspNetUser { get; set; }
    }
}
