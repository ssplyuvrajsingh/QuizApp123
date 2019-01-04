﻿using QuizApp.Models;
using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace QuizApp.Controllers
{
    [Authorize]
    public class QuizController : ApiController
    {
        #region Get Quiz

        //POST api/Quiz/GetQuiz
        [HttpPost]
        public ResultClass GetQuiz()
        {
            try
            {
                QuizBinding quiz = new QuizBinding();
                return new ResultClass()
                {
                    Data = quiz.GetQuiz(),
                    Message = "Data found successfully",
                    Result = true
                };
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

        #region Set Quiz

        //POST api/Quiz/SetQuiz
        [HttpPost]
        public ResultClass SetQuiz(QuizBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Result = false,
                        Message = "Please send required fields"
                    };
                }
                QuizBinding quiz = new QuizBinding();
                return new ResultClass()
                {
                    Data = quiz.SetQuiz(model),
                    Message = "Set quiz successfully",
                    Result = true
                };
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
        public ResultClass GetQuizQuestions(QuizQuestionBindingModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    QuizBinding quiz = new QuizBinding();
                    return new ResultClass()
                    {
                        Data = quiz.GetQuizQuestions(model.QuizId, model.UserId),
                        Message = "Data found successfully",
                        Result = true
                    };
                }
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