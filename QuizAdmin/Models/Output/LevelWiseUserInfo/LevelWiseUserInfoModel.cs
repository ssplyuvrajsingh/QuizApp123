using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class LevelWiseUserInfoModel
    {
        QuizAppEntities db = new QuizAppEntities();

        #region Get Level User Information
        public List<LevelUserInfoResult> GetLevelWiseUserInfo()
        {
            var data = (from u in db.Users
                        join au in db.AspNetUsers on u.UserID equals au.Id
                        where u.UserID == au.Id
                        select new LevelUserInfoResult
                        {
                            UserId = u.UserID,
                            Name = u.Name,
                            PhoneNumber = au.PhoneNumber,
                            ReferalCode = u.ReferalCode
                        }).ToList();
            var UserData = new List<LevelUserInfoResult>();
            foreach (var item in data)
            {
                var Lvl = db.LevelEarnings.Where(x => x.UserID == item.UserId).FirstOrDefault();
                if (Lvl != null && Lvl.Level1Users != null)
                {
                    int Count = Convert.ToInt32(Lvl.Level1Users + Lvl.Level2Users + Lvl.Level3Users + Lvl.Level4Users + Lvl.Level5Users + Lvl.Level6Users + Lvl.Level7Users + Lvl.Level8Users + Lvl.Level9Users + Lvl.Level10Users);
                    item.TotalUser = Count;
                    UserData.Add(item);
                }
                else
                {
                    item.TotalUser = 0;
                    UserData.Add(item);
                }
            }
            return UserData;
        }
        #endregion

        #region Total User
        public LevelUserInfoResult TotalUsers()
        {
            var LevelUsers = db.LevelEarnings.ToList();
            int totalLevelsUsers = 0;
            foreach(var Lvl in LevelUsers)
            {
                  totalLevelsUsers = totalLevelsUsers + Convert.ToInt32(Lvl.Level1Users + Lvl.Level2Users + Lvl.Level3Users + Lvl.Level4Users + Lvl.Level5Users + Lvl.Level6Users + Lvl.Level7Users + Lvl.Level8Users + Lvl.Level9Users + Lvl.Level10Users);
            }
            return new LevelUserInfoResult()
            {
                TotalUser = totalLevelsUsers
            };
        }
        #endregion
    }
}