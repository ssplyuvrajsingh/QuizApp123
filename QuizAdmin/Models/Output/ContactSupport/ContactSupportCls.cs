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
            var data = db.ContactSupports.OrderByDescending(x=>x.CreateDate).Select(x => new ContactSupportModel() {
                ContactSupportId=x.ContactSupportId,
                UserId=x.UserId,
                Mobile=x.Mobile,
                UserMessage=x.UserMessage
            }).ToList();

            return data;
            //var result = new List<ContactSupportModel>();
            //var res = new ContactSupportModel();
            //if(data.Any())
            //{
            //    foreach(var item in data)
            //    {
            //        res.ContactSupportId = item.ContactSupportId;
            //        res.UserId = item.UserId;
            //        res.Mobile = item.Mobile;
            //        res.UserMessage = item.UserMessage;
            //        result.Add(res);
            //    }
            //    return result;
            //}
            //else
            //{
            //    return null;
            //}
        }
        #endregion

    }
}