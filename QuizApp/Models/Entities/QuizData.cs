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
    
    public partial class QuizData
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuizData()
        {
            this.QuizPlayers = new HashSet<QuizPlayer>();
            this.QuizQuestions = new HashSet<QuizQuestion>();
        }
    
        public System.Guid QuizID { get; set; }
        public string QuizTitle { get; set; }
        public string PlayingDescriptionImg { get; set; }
        public string QuizBannerImage { get; set; }
        public Nullable<int> MaxPoint { get; set; }
        public Nullable<int> MinPoint { get; set; }
        public int WinPrecentage { get; set; }
        public Nullable<int> NoOfQuestion { get; set; }
        public Nullable<bool> isActive { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public System.DateTime CreatedDate { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuizPlayer> QuizPlayers { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuizQuestion> QuizQuestions { get; set; }
    }
}
