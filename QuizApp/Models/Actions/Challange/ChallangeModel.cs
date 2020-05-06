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
        public bool Status { get; set; }
    }

    public class AddUserModel
    {
        public List<SearchModel> AddUsers { get; set; }
        public int MinimumChallangerUsers { get; set; }
    }

    public class SearchKeyModel
    {
        public string UserID { get; set; }
        public int ChallangeId { get; set; }
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
    public class ChallangeListsModel
    {
        public List<ChallangesListModel> challangesLists { get; set; }
        public int TotalPoints { get; set; }
        public int MinimumChallangerUsers { get; set; }
        public int AdminPoints { get; set; }
        public int MinimumEntryPoints { get; set; }
    }
    #endregion

    #region Winner Users List
    public class WinnerUsersModel
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public int ChallangeId { get; set; }
        public string WinDate { get; set; }
    }
    #endregion

    #region Winner User Details
    public class WinnerDetailsModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public int Points { get; set; }
        public Nullable<bool> IsWinner { get; set; }
    }
    #endregion

    #region Saved Challange List
    public class SavedChallangeModel
    {
        public int ChallangeId { get; set; }
        public string StartDateTime { get; set; }
        public string UserId { get; set; }
    }
    #endregion
    #region Request Challange List
    public class RequestChallangeModel
    {
        public int ChallangeId { get; set; }
        public string StartDateTime { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
    }
    #endregion

    public class WinnerUserModel
    {
        public string ChallangeStartDateTime { get; set; }
        public string Phone { get; set; }
        public bool IsStatus { get; set; }
        public string Name { get; set; }
    }
}