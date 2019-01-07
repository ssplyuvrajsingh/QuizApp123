using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
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
                model.QuizID = Guid.NewGuid();
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

        #region Quiz Answer Add Edit Delete
        public List<QuizQuestion> getQuizAnswer()
        {
            return db.QuizQuestions.ToList();
        }

        public QuizQuestion getQuizAnswerById(int QuizQuestionId)
        {
            return db.QuizQuestions.Where(a => a.QuizQuestionID == QuizQuestionId).FirstOrDefault();
        }

        public bool addUpdateQuizAnswer(QuizQuestion model)
        {
            var old = db.QuizQuestions.Where(a => a.QuizQuestionID == model.QuizQuestionID).FirstOrDefault();
            if (old == null)
            {
                model.CreatedDate = DateTime.Now;
                db.QuizQuestions.Add(model);
                db.SaveChanges();
                return true;
            }
            else
            {
                db.Entry(old).CurrentValues.SetValues(model);
                return db.SaveChanges() > 0;
            }
        }

        public bool deleteQuizAnswer(int id)
        {
            var old = db.QuizQuestions.Where(a => a.QuizQuestionID == id).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            db.QuizQuestions.Remove(old);
            db.SaveChanges();
            return true;
        }
        #endregion
    }
}