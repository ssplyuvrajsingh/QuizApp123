using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace QuizApp.Models
{
    public class FCMPushNotification
    {
        public static FCMResponse SendNotificationFromFirebaseCloud(string to, string ChallengeDateTime)
        {
            var result = "-1";
            var webAddr = "https://fcm.googleapis.com/fcm/send";
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(webAddr);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Headers.Add(HttpRequestHeader.Authorization, "key=AAAAbAr21wQ:APA91bGSjUkae6WjEOtYhpUeE2NdwfJXFk3wQ3ld7XRb3xhLTP1-eWi5IEIdMcUC9XC-vOapP1yBUndYA5qFd8lhwSUgP1kXADqR5oz630-QIrf_v70UqcDH8jk9bSplRBaLSoJ2fi9v");
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string strNJson = "{\"to\": \"" + to + "\",\"notification\": {\"body\": \"Challange will start soon\",\"title\":\"Challenge starting soon :" + "" + ChallengeDateTime + "" + "\",\"content_available\":\"" + true + "\",\"priority\":\"high\",\"pushType\":\"user_challenge\"},\"data\":{\"body\": \"Challange will start soon\",\"title\":\"Challenge starting soon :" + ChallengeDateTime + "\",\"content_available\":\"" + true + "\",\"priority\":\"high\"}}";
                streamWriter.Write(strNJson);
                streamWriter.Flush();
            }

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                result = streamReader.ReadToEnd();
            }
            var FCMResponse = JsonConvert.DeserializeObject<FCMPushNotification.FCMResponse>(result);
            return FCMResponse;
        }

        public class FCMResponse
        {
            public long multicast_id { get; set; }
            public int success { get; set; }
            public int failure { get; set; }
            public int canonical_ids { get; set; }
            public List<FCMResult> results { get; set; }
        }
        public class FCMResult
        {
            public string message_id { get; set; }
        }
    }
}