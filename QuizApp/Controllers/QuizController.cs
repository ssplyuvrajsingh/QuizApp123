using QuizApp.Models;
using QuizApp.Models.Entities;
using QuizApp.Models.Input.Quiz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Configuration;


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
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
                {
                    QuizBinding quiz = new QuizBinding();
                    var data = quiz.GetQuiz(model.UserId);
                    if (data != null)
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
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        QuizBinding quiz = new QuizBinding();
                        var data = quiz.GetQuizQuestions(model.QuizId, model.UserId);
                        if (data != null)
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
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];

                    //model.ciphertoken = security.OpenSSLEncrypt(model.ciphertoken, secretKey);
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus =security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        QuizBinding quiz = new QuizBinding();
                        return new ResultClass()
                        {
                            Data = quiz.StartGame(model),
                            Message = "Data found successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
                    }
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus =  security.CheckDecypt(plainText);
                    if (isStatus)
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
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        QuizBinding quiz = new QuizBinding();
                        return new ResultClass()
                        {
                            Data = quiz.EndGame(model),
                            Message = "Game end successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
                    }
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
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        QuizBinding quiz = new QuizBinding();
                        return new ResultClass()
                        {
                            Data = quiz.GetScoreByQuiz(model),
                            Message = "Data found successfully",
                            Result = true
                        };
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
                    }
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
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
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
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
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
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus = security.CheckDecypt(plainText);
                if (isStatus)
                {
                    QuizBinding quizBinding = new QuizBinding();
                    var data = quizBinding.GetLevelBaseEarningAmount(model.UserID);
                    if (data != null)
                    {
                        result = new ResultClass()
                        {
                            Data = data,
                            Message = "Data Found",
                            Result = true
                        };
                    }
                }
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
                        Result = false
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
                quizBinding.LevelFirebaseUpdates(data);
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

        #region Get Level Wise User Information 
        [HttpPost]
        public ResultClass GetLevelWiseUserInformation(LevelWiseModel model)
        {
            ResultClass result = null;
            try
            {
                //TODO: Decrypt the encrypted value
                var security = new Security();
                var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                //Check Secret Code
                bool isStatus =  security.CheckDecypt(plainText);
                if (isStatus)
                {
                    QuizBinding quizBinding = new QuizBinding();
                    var data = quizBinding.GetLevelWiseUserInformation(model);
                    if (data.Any())
                    {
                        result = new ResultClass()
                        {
                            Data = data,
                            Message = "Data Found",
                            Result = true
                        };
                    }
                    else
                    {
                        result = new ResultClass()
                        {
                            Data = data,
                            Message = "Data Not Found",
                            Result = false
                        };
                    }
                }
                else
                {
                    return new ResultClass()
                    {
                        Message = "Timeout Error",
                        Result = false
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

        #region Top Ten Results on QuizId
        [HttpPost]
        public ResultClass TopTenResultsonQuizId(QuizIDModel model)
        {
            ResultClass result = null;
            try
            {
                if(!ModelState.IsValid)
                {
                    return new ResultClass()
                    {
                        Message = "Please send all required fields",
                        Result = false
                    };
                }
                else
                {
                    //TODO: Decrypt the encrypted value
                    var security = new Security();
                    var secretKey = ConfigurationManager.AppSettings["SecurityKey"];
                    var plainText = security.OpenSSLDecrypt(model.ciphertoken, secretKey);
                    //Check Secret Code
                    bool isStatus = security.CheckDecypt(plainText);
                    if (isStatus)
                    {
                        QuizBinding quizBinding = new QuizBinding();
                        var data = quizBinding.TopTenResultsonQuizId(model);
                        if (data.Any())
                        {
                            result = new ResultClass()
                            {
                                Data = data,
                                Message = "Data Found",
                                Result = true
                            };
                        }
                        else
                        {
                            result = new ResultClass()
                            {
                                Data = null,
                                Message = "Data Not Found",
                                Result = false
                            };
                        }
                    }
                    else
                    {
                        return new ResultClass()
                        {
                            Message = "Timeout Error",
                            Result = false
                        };
                    }
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
    }
}