using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class RepoCaption
    {
        #region Entities Declration
        QuizAppEntities db = new QuizAppEntities();
        #endregion

        #region Get Caption List
        public List<CaptionModel> GetCaption()
        {
            return db.Captions.OrderByDescending(x => x.CreateDate).Select(x => new CaptionModel()
            {
                Id = x.Id,
                Title = x.Title,
                Url = x.Url
            }).ToList();
        }
        #endregion

        #region Get Caption by Id
        public CaptionModel GetCaptionById(int Id)
        {
            return db.Captions.Where(x => x.Id == Id).Select(x => new CaptionModel()
            {
                Id = x.Id,
                Title = x.Title,
                Url = x.Url
            }).FirstOrDefault();
        }
        #endregion

        #region Add Edit Caption
        public bool AddCaption(CaptionModel model)
        {
            var data = db.Captions.Where(x => x.Id == model.Id).FirstOrDefault();
            if (data == null)
            {
                db.Captions.Add(new Caption()
                {
                    Title = model.Title,
                    Url = model.Url,
                    CreateDate = DateTime.Now
                });
                return db.SaveChanges() > 0;
            }
            else
            {
                data.Title = model.Title;
                data.Url = model.Url;
                db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete Caption
        public bool DeleteCaption(int Id)
        {
            var data = db.Captions.Where(x => x.Id == Id).FirstOrDefault();
            if(data!=null)
            {
                db.Captions.Remove(data);
                db.SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}