using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class RepoSupport
    {
        #region Db Declaration
        QuizAppEntities db = new QuizAppEntities();
        #endregion

        #region Support List
        public List<SupportModel> GetSupportsList()
        {
            return db.Supports.OrderByDescending(x => x.CreatedDate).Select(s => new SupportModel() {
                UserName=s.UserName,
                PhoneNumber=s.PhoneNumber,
                UserQuery=s.UserQuery
            }).ToList();
        }

        #endregion

        #region Add Support Query
        public bool AddSupportQuery(UserSupportModel model)
        {
           var query= new Support()
            {
                UserName = model.UserName,
                PhoneNumber = model.PhoneNumber,
                UserQuery = model.UserQuery,
                CreatedDate= DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00)
           };
            db.Supports.Add(query);
            return db.SaveChanges() > 0;
        }

        #endregion
    }
}