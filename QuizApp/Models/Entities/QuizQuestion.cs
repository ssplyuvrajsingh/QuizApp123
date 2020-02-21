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
    
    public partial class QuizQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuizQuestion()
        {
            this.UserAnswers = new HashSet<UserAnswer>();
        }
    
        public int QuizQuestionID { get; set; }
        public System.Guid QuizID { get; set; }
        public string Question { get; set; }
        public string ImageUrl { get; set; }
        public string Option1 { get; set; }
        public string Option2 { get; set; }
        public string Option3 { get; set; }
        public string Option4 { get; set; }
        public string CorrectOption { get; set; }
        public Nullable<int> MaxTime { get; set; }
        public Nullable<int> QuestionPoint { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserAnswer> UserAnswers { get; set; }
        public virtual QuizData QuizData { get; set; }
    }
}
