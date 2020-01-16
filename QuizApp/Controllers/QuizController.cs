using QuizApp.Models;
using QuizApp.Models.Entities;
using QuizApp.Models.Input.Quiz;
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
        public ResultClass GetQuiz(UserModel model)
        {
            try
            {
                QuizBinding quiz = new QuizBinding();
                var data = quiz.GetQuiz(model.UserId);
                if (data.Count() > 0)
                {
                    return new ResultClass()
                    {
                        Data = data,
                        Message = "Data found successfully",
                        Result = true
                    };
                }
                else
                {
                    return new ResultClass()
                    {
                        Data = data,
                        Message = "Data not found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new ResultClass()
                {
                    Result = false,
                    Message = ex.Message + "---" + ex.StackTrace,
                    Data = null
                };
            }
        }

        #endregion

        #region Get Quiz Questions

        //POST api/Quiz/GetQuizQuestions
        [HttpPost]
        public ResultClass GetQuizQuestions(QuizQuestionBindingModel model)
        {
            ResultClass ResultClass = new ResultClass();
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
                    var data = quiz.GetQuizQuestions(model.QuizId, model.UserId);
                    if(data!=null)
                    {
                        ResultClass = new ResultClass()
                        {
                            Data = data,
                            Message = "Data found successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        ResultClass = new ResultClass()
                        {
                            Data = null,
                            Message = "Data Not Found",
                            Result = false
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                ResultClass = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return ResultClass;
        }

        #endregion

        #region Start Game

        [HttpPost]
        public ResultClass StartGame(StartGameBindingModel model)
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
                        Data = quiz.StartGame(model),
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

        #region Set Final Result

        //POST api/Quiz/SetFinalResult
        [HttpPost]
        public ResultClass SetFinalResult(QuizPlayerResult model)
        {
            ResultClass resultClass = null;
            try
            {
                if (model.QuizID != null && model.UserID != null)
                {
                    QuizBinding quizBinding = new QuizBinding();
                    var data = quizBinding.SetQuizePlayer(model);
                    if (data != null)
                    {
                        resultClass = new ResultClass()
                        {
                            Data = data,
                            Message = "Save Quize Player Successfully",
                            Result = true
                        };
                    }
                }
                else
                {
                    resultClass = new ResultClass()
                    {
                        Result = false,
                        Message = "Please send required fields"
                    };
                }
            }
            catch (Exception ex)
            {
                resultClass = new ResultClass()
                {
                    Result = false,
                    Message = ex.Message,
                };
            }
            return resultClass;
        }
        #endregion

        #region End Game

        [HttpPost]
        public ResultClass EndGame(EndGameBindingModel model)
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
                        Data = quiz.EndGame(model),
                        Message = "Game end successfully",
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

        #region GetScoreByQuiz

        [HttpPost]
        public ResultClass GetScoreByQuiz(GetScoreByQuiz model)
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
                        Data = quiz.GetScoreByQuiz(model),
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

        #region Get Wallet Information
        // POST api/Quiz/GetWalletInformation
        [HttpPost]
        public ResultClass GetWalletInformation(UserModel model)
        {
            ResultClass resultClass = null;
            try
            {
                QuizBinding quizBinding = new QuizBinding();
                var data = quizBinding.GetWalletInfo(model);
                if (data != null)
                {
                    resultClass = new ResultClass()
                    {
                        Data = data,
                        Message = "Data Found",
                        Result = true
                    };
                }
                else
                {
                    resultClass = new ResultClass()
                    {
                        Data = data,
                        Message = "Data Not Found",
                        Result = false
                    };
                }
            }
            catch (Exception ex)
            {
                resultClass = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return resultClass;
        }
        #endregion

        #region Get Level Base Earning Amount
        [HttpPost]
        public ResultClass GetLevelBaseEarningAmount(LevelUsers model)
        {
            ResultClass result = null;
            try
            {
                QuizBinding quizBinding = new QuizBinding();
                var data = quizBinding.GetLevelBaseEarningAmount(model.UserID);
                if(data!=null)
                {
                    result = new ResultClass()
                    {
                        Data = data,
                        Message = "Data Found",
                        Result = true
                    };
                }
            }
            catch(Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion

        #region Add Level Base Earning Amount
        [AllowAnonymous]
        [HttpGet]
        public ResultClass AddLevelBaseEarningAmount()
        {
            ResultClass result = null;
            try
            {
                QuizBinding quizBinding = new QuizBinding();
                var jsonFilePath = HttpContext.Current.Server.MapPath("~/Models/JsonFile/LevelEarningMasterUser.json");
                var data = quizBinding.AddLevelBaseEarningAmount(jsonFilePath);
                if (data)
                {
                    result = new ResultClass()
                    {
                        Data = data,
                        Message = "added user level",
                        Result = true
                    };
                }
            }
            catch (Exception ex)
            {
                result = new ResultClass()
                {
                    Data = null,
                    Message = ex.Message,
                    Result = false
                };
            }
            return result;
        }
        #endregion
    }
}