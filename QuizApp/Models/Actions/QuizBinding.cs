using QuizApp.Models.Entities;
using QuizApp.Models.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models.Actions
{
    public class QuizBinding
    {
        public ResultClass AddQuiz(QuizBindingModel model)
        {
            ResultClass result = new ResultClass();
            try
            {
                using (QuizAppEntities entities = new QuizAppEntities())
                {
                    QuizData quizData = new QuizData()
                    {
                        CreatedDate = DateTime.Now,
                        MaxPoint = model.MaxPoint,
                        MinPoint = model.MinPoint,
                        NoOfQuestion = model.NoOfQuestion,
                        PlayingDescriptionImg = model.PlayingDescriptionImg,
                        QuizBannerImage = model.QuizBannerImage,
                        QuizDate = DateTime.Now,
                        QuizID = model.QuizId,
                        QuizTitle = model.QuizTitle,
                        WinPrecentage = model.WinPrecentage
                    };

                    entities.QuizDatas.Add(quizData);
                    int updatedRow = entities.SaveChanges();

                    if (updatedRow >= 1)
                    {
                        result.Result = true;
                        result.Message = "Quiz submited successfullt";
                    }
                    else
                    {
                        result.Result = false;
                        result.Message = "Quiz submited failure";
                    }
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Result = false;
                result.Message = ex.Message;
                return result;
            }
        }
    }
}