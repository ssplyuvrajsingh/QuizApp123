using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAppAdmin.Models
{
    public class RepoQuiz
    {
        QuizAppEntities db = new QuizAppEntities();

        #region Quiz Add Edit Delete
        public List<QuizData> getQuiz()
        {
            return db.QuizDatas.ToList();
        }
        public QuizData getQuizById(Guid QuizId)
        {
            return db.QuizDatas.Where(a => a.QuizID == QuizId).FirstOrDefault();
        }
        public bool addUpdateQuiz(QuizData model)
        {
            var old = db.QuizDatas.Where(a => a.QuizID == model.QuizID).FirstOrDefault();
            if (old == null)
            {
                model.CreatedDate = DateTime.Now;
                db.QuizDatas.Add(model);
                db.SaveChanges();
                return true;
            }
            else
            {
                db.Entry(old).CurrentValues.SetValues(model);
                return db.SaveChanges() > 0;
            }
        }

        public bool deleteQuiz(Guid id)
        {
            var old = db.QuizDatas.Where(a => a.QuizID == id).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            db.QuizDatas.Remove(old);
            db.SaveChanges();
            return true;
        }
        #endregion
    }
}