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
        #region Get Quiz

        //POST api/Quiz/GetQuiz
        [HttpPost]
        [AllowAnonymous]
        [Route("GetQuiz")]
        public ResultClass GetQuiz()
        {
            try
            {
                QuizBinding quiz = new QuizBinding();

                var result = quiz.GetQuiz();

                return result;
            }
            catch (Exception ex)
            {
                ResultClass result = new ResultClass();
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }

        #endregion

        #region Get Quiz Questions

        //POST api/Quiz/GetQuizQuestions
        [HttpPost]
        [AllowAnonymous]
        [Route("GetQuizQuestions")]
        public ResultClass GetQuizQuestions(string QuizId)
        {
            try
            {
                QuizBinding quiz = new QuizBinding();

                var result = quiz.GetQuiz();

                return result;
            }
            catch (Exception ex)
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