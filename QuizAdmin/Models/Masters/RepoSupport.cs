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
    }
}