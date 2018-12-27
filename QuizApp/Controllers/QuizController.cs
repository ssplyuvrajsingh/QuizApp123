using QuizApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QuizApp.Controllers
{
    public class QuizController : ApiController
    {
        public ResultClass HelloWorld()
        {
            return new ResultClass()
            {
                Message = "Hello World!",
                Status = true
            };
        }
    }
}
