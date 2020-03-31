using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class AdminProfile
    {
        QuizAppEntities db = new QuizAppEntities();

        #region Get Admin Profile Information
        public AdminInfoResult AdminProfileInfo(string PhoneNumber)
        {
            var data = db.AspNetUsers.Where(a => a.PhoneNumber == PhoneNumber).FirstOrDefault();
            var adminData = db.Users.Where(a => a.UserID == data.Id).FirstOrDefault();
            return new AdminInfoResult()
            {
                Name = adminData.Name,
                PhoneNumber = data.PhoneNumber,
                ReferalCode = adminData.ReferalCode
            };
        }
        #endregion

        #region Get Level Base Earning Amount
        public List<LevelEarningModel> GetActiveUsersLevelWise(string Users)
        {
            var data = db.LevelEarnings.Where(x => x.UserID == Users).FirstOrDefault();
            List<LevelEarningModel> levelEarnings = new List<LevelEarningModel>();
            for (int i = 1; i <= 10; i++)
            {
                var lvl = new LevelEarningModel();
                switch (i)
                {
                    case 1:
                        lvl.Level = 1;
                        lvl.Title = "Level 1";
                        lvl.Activeuser = data != null ? data.Level1Users != null ? (int)data.Level1Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level1 != null ? (double)data.Level1 : 0 : 0;
                        break;

                    case 2:
                        lvl.Level = 2;
                        lvl.Title = "Level 2";
                        lvl.Activeuser = data != null ? data.Level2Users != null ? (int)data.Level2Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level2 != null ? (double)data.Level2 : 0 : 0;
                        break;
                    case 3:
                        lvl.Level = 3;
                        lvl.Title = "Level 3";
                        lvl.Activeuser = data != null ? data.Level3Users != null ? (int)data.Level3Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level3 != null ? (double)data.Level3 : 0 : 0;
                        break;
                    case 4:
                        lvl.Level = 4;
                        lvl.Title = "Level 4";
                        lvl.Activeuser = data != null ? data.Level4Users != null ? (int)data.Level4Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level4 != null ? (double)data.Level4 : 0 : 0;
                        break;
                    case 5:
                        lvl.Level = 5;
                        lvl.Title = "Level 5";
                        lvl.Activeuser = data != null ? data.Level5Users != null ? (int)data.Level5Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level5 != null ? (double)data.Level5 : 0 : 0;
                        break;
                    case 6:
                        lvl.Level = 6;
                        lvl.Title = "Level 6";
                        lvl.Activeuser = data != null ? data.Level6Users != null ? (int)data.Level6Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level6 != null ? (double)data.Level6 : 0 : 0;
                        break;
                    case 7:
                        lvl.Level = 7;
                        lvl.Title = "Level 7";
                        lvl.Activeuser = data != null ? data.Level7Users != null ? (int)data.Level7Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level7 != null ? (double)data.Level7 : 0 : 0;
                        break;
                    case 8:
                        lvl.Level = 8;
                        lvl.Title = "Level 8";
                        lvl.Activeuser = data != null ? data.Level8Users != null ? (int)data.Level8Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level8 != null ? (double)data.Level8 : 0 : 0;
                        break;
                    case 9:
                        lvl.Level = 9;
                        lvl.Title = "Level 9";
                        lvl.Activeuser = data != null ? data.Level9Users != null ? (int)data.Level9Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level9 != null ? (double)data.Level9 : 0 : 0;
                        break;
                    case 10:
                        lvl.Level = 10;
                        lvl.Title = "Level 10";
                        lvl.Activeuser = data != null ? data.Level10Users != null ? (int)data.Level10Users : 0 : 0;
                        lvl.Amount = data != null ? data.Level10 != null ? (double)data.Level10 : 0 : 0;
                        break;
                }
                levelEarnings.Add(lvl);
            }
            return levelEarnings;
        }
        #endregion

        #region Total Active User 
        public LevelEarningModel TotalActiveUser()
        {
            var activeUsers = db.Users.ToList();
            int data = activeUsers.Where(x => x.LastActiveDate != null && (x.LastActiveDate.Value).Date == (DateTime.Now.AddDays(-1)).Date || (x.LastActiveDate.Value).Date == (DateTime.Now).Date).Count();
            return new LevelEarningModel()
            {
                Activeuser=data
            };

        }
        #endregion
    }
}