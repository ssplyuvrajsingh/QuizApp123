using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizAdmin.Models
{
    public class RepoQuiz
    {
        QuizAppEntities db = new QuizAppEntities();

        #region Quiz List
        public List<QuizData> getQuiz()
        {
            return db.QuizDatas.Where(x => x.IsDeleted == null || x.IsDeleted.Value == false).OrderByDescending(x => x.CreatedDate).ToList();
        }
        #endregion

        #region Get Quiz By Id
        public QuizData getQuizById(Guid QuizId)
        {
            return db.QuizDatas.Where(a => (a.IsDeleted == null || a.IsDeleted.Value == false) && a.QuizID == QuizId).FirstOrDefault();
        }
        #endregion

        #region Add Update Quiz
        public bool addUpdateQuiz(QuizData model)
        {
            var old = db.QuizDatas.Where(a => a.QuizID == model.QuizID).FirstOrDefault();
            if (old == null)
            {
                model.QuizID = Guid.NewGuid();
                model.CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
                model.isActive = false;
                db.QuizDatas.Add(model);
                db.SaveChanges();
                return true;
            }
            else
            {
                model.isActive = old.isActive;
                db.Entry(old).CurrentValues.SetValues(model);
                int value = db.SaveChanges();
                return true;
            }
        }
        #endregion

        #region Delete Quiz
        public bool deleteQuiz(Guid id)
        {
            var old = db.QuizDatas.Where(a => a.QuizID == id).FirstOrDefault();
            if (old != null)
            {
                old.IsDeleted = true;
                db.SaveChanges();

                var quizQuestion = db.QuizQuestions.Where(x => x.QuizID == id).ToList();
                if (quizQuestion.Any())
                {
                    quizQuestion.ForEach(f => f.IsDeleted = true);
                    db.SaveChanges();
                }
                return true;
            }
            return false;
        }
        #endregion

        #region ActiveQuiz
        public string ActiveQuiz(Guid id)
        {
            var old = db.QuizDatas.Where(a => (a.IsDeleted == null || a.IsDeleted.Value == false) && a.QuizID == id).FirstOrDefault();
            if (old != null)
            {
                if ((bool)old.isActive)
                {
                    old.isActive = false;
                    db.SaveChanges();
                    return "De-Active";
                }
                else
                {
                    old.isActive = true;
                    db.SaveChanges();
                    return "Active";
                }
            }
            else
            {
                return "False";
            }
        }
        #endregion

        #region Get Quiz Answer
        public List<QuizQuestion> getQuizAnswer()
        {
            return db.QuizQuestions.Where(x => x.IsDeleted == null || x.IsDeleted.Value == false).ToList();
        }
        #endregion

        #region Get Quiz Answer By Id
        public QuizQuestion getQuizAnswerById(int QuizQuestionId)
        {
            return db.QuizQuestions.Where(a => (a.IsDeleted == null || a.IsDeleted.Value == false) && a.QuizQuestionID == QuizQuestionId).FirstOrDefault();
        }

        public bool addUpdateQuizAnswer(QuizQuestion model)
        {
            var old = db.QuizQuestions.Where(a => a.QuizQuestionID == model.QuizQuestionID).FirstOrDefault();
            if (old == null)
            {
                model.CreatedDate = DateTime.UtcNow.AddHours(5.00).AddMinutes(30.00);
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
        #endregion

        #region Delete Quiz Answer
        public bool deleteQuizAnswer(int id)
        {
            var old = db.QuizQuestions.Where(a => a.QuizQuestionID == id).FirstOrDefault();
            if (old == null)
            {
                return false;
            }
            old.IsDeleted = true;
            db.SaveChanges();
            return true;
        }
        #endregion

        #region Users Quiz List
        public List<QuizUserCount> UsersQuizList(string id)
        {
            var old = db.QuizPlayers.Where(a => a.UserID == id).GroupBy(a => a.QuizID).ToList();
            List<QuizUserCount> quizcount = new List<QuizUserCount>();
            foreach (var quiz in old)
            {
                quizcount.Add(new QuizUserCount()
                {
                    Quiztitle = quiz.FirstOrDefault().QuizData.QuizTitle,
                    QuizCount = quiz.Count()
                });
            }
            return quizcount;
        }
        #endregion
    }
}