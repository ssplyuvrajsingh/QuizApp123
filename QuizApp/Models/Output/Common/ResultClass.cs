using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class ResultClass
    {
        public string Message { get; set; }
        public bool Result { get; set; }
        public object Data { get; set; }
    }

    public class LoginErrorMessage
    {
        public string error_description { get; set; }
    }

    public class LoginSuccessMessage
    {
        public string access_token { get; set; }
    }

    public class GetUserIdPasswordResponse
    {
        public string UserId { get; set; }
        public string Password { get; set; }
    }
}