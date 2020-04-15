using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models

{
    #region Screen First Model
    public class ChallangeModel
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Nullable<int> ChallangeId { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public Nullable<System.DateTime> StartDateTime { get; set; }
        public Nullable<bool> IsCompleted { get; set; }
        public Nullable<System.DateTime> CompletedDateTime { get; set; }
        public Nullable<bool> IsWinner { get; set; }
        public Nullable<int> Points { get; set; }
    }

    public class ChallangeIdModel
    {
        public Nullable<int> ChallangeId { get; set; }
    }
    #endregion

    #region Second Screen Search Users
    public class SearchModel
    {
        public string UserID { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
    }

    public class SearchKeyModel
    {
        public string UserID { get; set; }
        public string Search { get; set; }
    }
    #endregion

    #region Five Screen
    public class ChallangesListModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Nullable<int> ChallangeId { get; set; }
        public Nullable<bool> IsAdmin { get; set; }
        public Nullable<bool> IsAccepted { get; set; }
        public Nullable<int> Points { get; set; }
    }
    #endregion
}