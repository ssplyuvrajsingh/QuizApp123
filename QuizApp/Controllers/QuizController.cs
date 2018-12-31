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
        public ResultClass Quiz(QuizBindingModel model)
        {
            try
            {
                ResultClass result = new ResultClass();

                string quizId = Guid.NewGuid().ToString();
                model.QuizId = quizId;

                QuizBinding quiz = new QuizBinding();

                result = quiz.AddQuiz(model);

                return result;
            }
            catch(Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        #endregion
    }
}