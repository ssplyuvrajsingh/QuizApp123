using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class GeneralFunctions
    {
        public string GetOTP()
        {
            return "1234";
        }

        private static Random random = new Random();
        public string GetReferalCode()
        {
            int length = random.Next(4, 11);

            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string referalCode = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var isExistReferalCode = entities.Users.Where(x => x.ReferalCode == referalCode);

                if (isExistReferalCode != null)
                {
                    GetReferalCode();
                }
            }

            return referalCode;
        }
    }
}