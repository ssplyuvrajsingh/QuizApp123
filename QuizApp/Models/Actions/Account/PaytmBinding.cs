﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace QuizApp.Models
{
    public class PaytmBinding
    {
        #region Paytm Binding
        public string PaytmResponse(string Phone, string PaymentText, string Amount,string Ip)
        {
            string merchantguid = "7f1c79d3-5386-47d7-ac46-707ae6126842";
            string orderid = DateTime.Now.Ticks.ToString();
            string AesKey = "ZBVhw3s0alzVds@k"; // 16 digits Merchant Key or Aes Key
            string saleswalletid = "b6961cdb-cc23-4d74-927e-5555f9ba52a2";
            //string phone = "9785507506";
            string postData = "{\"request\":{\"requestType\":\"null\",\"merchantGuid\":\"" + merchantguid + "\",\"merchantOrderId\":\"" + orderid + "\",\"salesWalletName\":\"\",\"salesWalletGuid\":\"" + saleswalletid + "\",\"payeeEmailId\":\"email@paytm.com\",\"payeePhoneNumber\":\"" + Phone + "\",\"payeeSsoId\":\"\",\"appliedToNewUsers\":\"N\",\"amount\":\""+ Amount + "\",\"currencyCode\":\"INR\"},\"metadata\":\""+ PaymentText + "\",\"ipAddress\":\""+ Ip +"\",\"operationType\":\"SALES_TO_USER_CREDIT\",\"platformName\":\"PayTM\"}";

            string checksum = paytm.CheckSum.generateCheckSumByJson(AesKey, postData);
            string uri = "https://trust.paytm.in/wallet-web/salesToUserCredit";

            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
            webRequest.Method = "POST";
            webRequest.Accept = "application/json";
            webRequest.ContentType = "application/json";
            webRequest.Headers.Add("mid", merchantguid);
            webRequest.Headers.Add("checksumhash", checksum);

            webRequest.ContentLength = postData.Length;
            try
            {
                using (StreamWriter requestWriter2 = new StreamWriter(webRequest.GetRequestStream()))
                {
                    requestWriter2.Write(postData);
                }

                //  This actually does the request and gets the response back;

                string responseData = string.Empty;

                using (StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
                {
                    responseData = responseReader.ReadToEnd();
                    return responseData;
                }
            }
            catch (WebException web)
            {
                HttpWebResponse res = web.Response as HttpWebResponse;
                Stream s = res.GetResponseStream();
                string message;
                using (StreamReader sr = new StreamReader(s))
                {
                    message = sr.ReadToEnd();
                    
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return "False";
        }
        #endregion
    }
}