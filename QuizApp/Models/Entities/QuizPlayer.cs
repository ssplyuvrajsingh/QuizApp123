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
    
    public partial class QuizPlayer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuizPlayer()
        {
            this.UserAnswers = new HashSet<UserAnswer>();
        }
    
        public int PlayerID { get; set; }
        public string UserID { get; set; }
        public System.Guid QuizID { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<bool> IsWon { get; set; }
        public Nullable<int> PointEarn { get; set; }
        public Nullable<System.DateTime> PlayedDate { get; set; }
        public Nullable<int> PercentageEarn { get; set; }
        public string Language { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual QuizData QuizData { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
    }
}
