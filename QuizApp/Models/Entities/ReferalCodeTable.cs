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
    
    public partial class ReferalCodeTable
    {
        public int ReferalId { get; set; }
        public string IPAddress { get; set; }
        public string UserAgent { get; set; }
        public string DeviceModel { get; set; }
        public Nullable<bool> IsUsed { get; set; }
        public string ReferalCode { get; set; }
    }
}