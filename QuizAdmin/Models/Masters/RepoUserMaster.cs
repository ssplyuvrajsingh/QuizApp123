using Microsoft.AspNet.Identity;
using QuizAdmin.Models.Output;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace QuizAdmin.Models
{
    public class RepoUserMaster
    {
        QuizAppEntities db = new QuizAppEntities();
        #region Quiz list
        public List<AllUser> getUser()
        {
            var allusers = (from u in db.Users
                            join au in db.AspNetUsers on u.UserID equals au.Id
                            where u.UserID == au.Id
                            select new AllUser
                            {
                                Name = u.Name,
                                Email = au.Email,
                                PhoneNumber = au.UserName,
                                Platform = u.Platform,
                                UserId = u.UserID
                            }).ToList();
            return allusers;
        }
        #endregion
    }
}