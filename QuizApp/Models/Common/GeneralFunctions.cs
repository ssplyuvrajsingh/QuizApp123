﻿using Newtonsoft.Json;
using QuizApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace QuizApp.Models
{
    public class GeneralFunctions
    {
        private static Random random = new Random();
        public static int GetOTP()
        {
            int length = random.Next(4, 4);

            string OTP = GenerateRandomOTP(length);
            return Convert.ToInt32(OTP);
        }
        public static string GenerateRandomOTP(int length)
        {
            //const string alphabetCharacters = "abcdefghijklmnopqrstuvwxyz";
            const string numCharacters = "0123456789";
            const string numCharacter = "123456789";
            StringBuilder Code = new StringBuilder(length);

            Code.Append(numCharacter[random.Next(numCharacter.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            return Code.ToString();
        }


        public static string GetReferalCode()
        {
            Random rnd = new Random();
            int length = random.Next(6, 6);

            string referalCode = RandomString(length);

            using (QuizAppEntities entities = new QuizAppEntities())
            {
                var isExistReferalCode = entities.Users.Where(x => x.ReferalCode == referalCode).FirstOrDefault();

                if (isExistReferalCode != null)
                {
                    referalCode = GetReferalCode();
                }
            }

            return referalCode;
        }

        public static string GETData(string url)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public static string GETDataNew(string url, string postData)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            var data = Encoding.ASCII.GetBytes(postData);
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = data.Length;
            request.UserAgent = "Mozilla/5.0 (Windows; U; Windows NT 6.0; en-US; rv:1.8.1.3) Gecko/20070309 Firefox/2.0.0.3";
            request.ProtocolVersion = HttpVersion.Version10;
            request.KeepAlive = true;
            request.AllowAutoRedirect = false;
            request.CachePolicy = new System.Net.Cache.RequestCachePolicy(System.Net.Cache.RequestCacheLevel.NoCacheNoStore);
            request.Accept = "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5";
            request.Timeout = 15000;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    return reader.ReadToEnd();
                }
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8"));
                    String errorText = reader.ReadToEnd();
                    // log errorText
                }
                throw;
            }
        }

        public static string RandomString(int length)
        {
            const string alphabetCharacters = "abcdefghijklmnopqrstuvwxyz";
            const string numCharacters = "0123456789";
            StringBuilder Code = new StringBuilder(length);
            //ABCDEFGHIJKLMNOPQRSTUVWXYZ

            // Format -> A NN AA N  
            Code.Append(alphabetCharacters[random.Next(alphabetCharacters.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            Code.Append(alphabetCharacters[random.Next(alphabetCharacters.Length)]);
            Code.Append(alphabetCharacters[random.Next(alphabetCharacters.Length)]);
            Code.Append(numCharacters[random.Next(numCharacters.Length)]);
            return Code.ToString();

            //const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            //return new string(Enumerable.Repeat(chars, 6)
            //  .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        #region Get Decimal value
        public string GetDecimalvalue(string amount)
        {
            double x;
            Double.TryParse(amount, out x);
            return x.ToString("0.00");
        }
        #endregion

        #region Get Point Redeem Value Check
        public bool PointReddemValueCheck(int Value)
        {
            int[] check = { 100, 200, 300, 400, 500, 600, 700, 800, 900, 1000 };
            bool data = false;
            foreach (var item in check)
            {
                if (Value == item)
                {
                    data = true;
                }
            }
            return data;
        }
        #endregion

        #region Get Earning Heads
        public EaningHeadModel getEarningHeads()
        {
            var jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "Models/JsonFile/LevelEarningMasterUser.json");
            EaningHeadModel earningHeads = new EaningHeadModel();
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                earningHeads = JsonConvert.DeserializeObject<EaningHeadModel>(json);
            }
            return earningHeads;
        }
        #endregion

        #region Get Allowed Users
        public string getAllowedUser()
        {
            var jsonFilePath = Path.Combine(HttpRuntime.AppDomainAppPath, "Models/JsonFile/LevelEarningMasterUser.json");
            AllowedUsers Users = new AllowedUsers();
            using (StreamReader r = new StreamReader(jsonFilePath))
            {
                string json = r.ReadToEnd();
                Users = JsonConvert.DeserializeObject<AllowedUsers>(json);
            }
            return Users.AllowUserId;
        }
        #endregion

        #region Return Temporary User
        public static int GetTemporaryUser(int Count)
        {
            int length = random.Next(4, 4);

            string OTP = GenerateRandomOTP(length);
            return Convert.ToInt32(OTP);
        }
        public static string GetRandomTemporaryUser(int length)
        {
            string num = string.Empty;
            for(int i=0;i<length;i++)
            {
                num =num + i;
            }
            //const string alphabetCharacters = "abcdefghijklmnopqrstuvwxyz";
            StringBuilder Code = new StringBuilder(1);

            Code.Append(num[random.Next(num.Length)]);
            return Code.ToString();
        }
        #endregion
    }
}