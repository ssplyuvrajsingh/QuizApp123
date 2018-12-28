using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuizApp.Controllers
{
    public class GeneralController : ApiController
    {
        #region Hello World
        public ResultClass HelloWorld()
        {

            try
            {
                return new ResultClass()
                {
                    Message = "Hello World",
                    Result = true
                };
            }
            catch (Exception ex)
            {
                return RepoUserLogs.SendExceptionMailFromController(this.ControllerContext.RouteData.Values["controller"].ToString(), this.ControllerContext.RouteData.Values["action"].ToString(), ex.Message, ex.StackTrace);

            }
        }

        #endregion
    }
}
