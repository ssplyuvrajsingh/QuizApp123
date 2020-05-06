using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class CrudRepo
    {
        #region Entities Declration
        QuizAppEntities db = new QuizAppEntities();
        #endregion

        #region Get Crud List
        public List<FilterWordModel> GetCrud()
        {
            return db.FilterWords.Select(x => new FilterWordModel()
            {
                Id = x.Id,
                FilterData = x.FilterData
            }).ToList();
        }
        #endregion

        #region Get Crud by Id
        public FilterWordModel GetCrudById(int Id)
        {
            return db.FilterWords.Where(x => x.Id == Id).Select(x => new FilterWordModel()
            {
                Id = x.Id,
                FilterData = x.FilterData
            }).FirstOrDefault();
        }
        #endregion

        #region Add Edit Caption
        public bool CrudAdd(FilterWordModel model)
        {
            var data = db.FilterWords.Where(x => x.Id == model.Id).FirstOrDefault();
            if (data == null)
            {
                db.FilterWords.Add(new FilterWord()
                {
                    FilterData=model.FilterData
                });
                return db.SaveChanges() > 0;
            }
            else
            {
                data.FilterData = model.FilterData;
                db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete Crud
        public bool DeleteCrud(int Id)
        {
            var data = db.FilterWords.Where(x => x.Id == Id).FirstOrDefault();
            if (data != null)
            {
                db.FilterWords.Remove(data);
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