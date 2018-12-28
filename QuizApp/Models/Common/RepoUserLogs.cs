using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class RepoUserLogs
    {
        #region Exception Handling
        public static ResultClass SendExceptionMailFromController(string controller, string action, string Message, string StackTrack)
        {
            SendMail("controller=" + controller + "<br/>action=" + action + "<br/>" + "Message=" + Message + "<br/>" + "StackTrack=" + StackTrack);
            return new ResultClass
            {
                Result = 0,
                Message = "Too many users in system please try again later."
            };
        }

        public static void SendExceptionMail(string Heading, string Message, string StackTrack)
        {
            SendMail("Heading=" + Heading + "<br/>Message =" + Message + "<br/>" + "StackTrack=" + StackTrack);
        }

        public static void SendMail(string Details)
        {
            MailSenderRepo sendmail = new MailSenderRepo();
            var dev = System.Configuration.ConfigurationManager.AppSettings["DeveloperMailId"].ToString();
            sendmail.MailSender(dev, Details, "AdminEaze Exception");
        }
        #endregion
    }
}