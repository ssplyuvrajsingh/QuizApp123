using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;

namespace QuizApp
{
    public class MailSenderRepo
    {
        public string MailSender(string to, string html, string subject)
        {
            string retVal;
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                msg.From =msg.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["MailSenderUserName"].ToString(), ConfigurationManager.AppSettings["MailSenderDisplayName"].ToString());
                msg.To.Add(to);
                msg.Subject = subject;
                msg.Body = html;
                msg.IsBodyHtml = true;
                smtpClient.Host = ConfigurationManager.AppSettings["MailHost"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"].ToString());
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailSenderUserName"].ToString(), ConfigurationManager.AppSettings["MailSenderPass"].ToString());
                smtpClient.EnableSsl = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableSsl"].ToString());
                smtpClient.Send(msg);
                retVal = "true";

            }
            catch (Exception ex)
            {
                // lblMsg.Text = ex.Message;
                retVal = ex.Message;
                //retVal += "--" + ex.StackTrace;
            }

            return retVal;
        }

        public string MailSender(string to, string html, string subject, string filePath, string ContentType)
        {
            string retVal;
            try
            {
                System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
                SmtpClient smtpClient = new SmtpClient();
                msg.From = new System.Net.Mail.MailAddress(ConfigurationManager.AppSettings["MailSenderUserName"].ToString(), ConfigurationManager.AppSettings["MailSenderDisplayName"].ToString());
                msg.To.Add(to);
                msg.Subject = subject;
                msg.Body = html;
                msg.IsBodyHtml = true;
                msg.Attachments.Add(new Attachment(filePath, ContentType));
                smtpClient.Host = ConfigurationManager.AppSettings["MailHost"].ToString();
                smtpClient.Port = Convert.ToInt32(ConfigurationManager.AppSettings["MailPort"].ToString());
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = new System.Net.NetworkCredential(ConfigurationManager.AppSettings["MailSenderUserName"].ToString(), ConfigurationManager.AppSettings["MailSenderPass"].ToString());
                smtpClient.EnableSsl = false;
                smtpClient.Send(msg);
                retVal = "true";

            }
            catch (Exception ex)
            {
                retVal = ex.Message;
            }

            return retVal;
        }

        public string MailHtml(string Details, string Username)
        {
            string str = "<table align='center' style='cursor: default; border-style: dashed; border-color: rgb(187, 187, 187); font-family: Verdana, Arial, Helvetica, sans-serif;'><tbody><tr><td data-mce-style='font-family: Helvetica Neue, Helvetica, Arial, sans-serif; font-size: 14px; color: #666;' style='color: rgb(102, 102, 102); font-family: 'Helvetica Neue', Helvetica, Arial, sans-serif; font-size: 14px; margin: 8px; cursor: text; border-style: dashed; border-color: rgb(187, 187, 187);'><div data-mce-style='text-align: left; background-color: #fff; max-width: 500px; border-top: 10px solid #0088cc; border-bottom: 3px solid #0088cc;' style='max-width: 500px; border-top-width: 10px; border-top-style: solid; border-top-color: rgb(0, 136, 204); border-bottom-width: 3px; border-bottom-style: solid; border-bottom-color: rgb(0, 136, 204);'><div data-mce-style='padding: 10px 20px; color: #000; font-size: 20px; background-color: #efefef; border-bottom: 1px solid #ddd;' style='padding: 10px 20px; color: rgb(0, 0, 0); font-size: 20px; background-color: rgb(239, 239, 239); border-bottom-width: 1px; border-bottom-style: solid; border-bottom-color: rgb(221, 221, 221);'>" + ConfigurationManager.AppSettings["sitename"].ToString() + "</div><div data-mce-style='padding: 20px; background-color: #fff; line-height: 18px;' style='padding: 20px; line-height: 18px;'>";
            str += "<p data-mce-style='text-align: center;'>Dear " + Username + "</p>";
            str += Details;
            str += "<div></div></td></tr></tbody></table>";
            return str;
        }
    }
}
