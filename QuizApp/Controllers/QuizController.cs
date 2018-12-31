using QuizApp.Models;
using QuizApp.Models.Actions;
using QuizApp.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QuizApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Quiz")]
    public class QuizController : Controller
    {
        #region Quiz

        //POST api/Account/Quiz
       [AllowAnonymous]
       [Route("Quiz")]
        public ActionResult GetQuiz()
        {
            try
            {
                QuizBinding quiz = new QuizBinding();

                var result = quiz.GetQuiz();

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion
    }
}