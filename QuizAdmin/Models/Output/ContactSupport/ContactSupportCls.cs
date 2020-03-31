using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class ContactSupportCls
    {
        QuizAppEntities db = new QuizAppEntities();

        #region Get Contect Support
        public List<ContactSupportModel> GetContectSupport()
        {
            var data = db.ContactSupports.OrderByDescending(x=>x.CreateDate).ToList();
            var result = new List<ContactSupportModel>();
            if(data.Any())
            {
                var res = new ContactSupportModel();
                foreach(var item in data)
                {
                    res.ContactSupportId = item.ContactSupportId;
                    res.UserId = item.UserId;
                    res.Mobile = item.Mobile;
                    res.UserMessage = item.UserMessage;
                    result.Add(res);
                }
                return result;
            }
            else
            {
                return null;
            }
        }
        #endregion

    }
}