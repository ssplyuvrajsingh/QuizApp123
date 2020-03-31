using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QuizApp.Models
{
    public class paytmResponse
    {
        public class Response
        {
            /// <summary>
            /// 
            /// </summary>
            public string walletSysTransactionId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string destination { get; set; }
        }
        public class Root
        {
            /// <summary>
            /// 
            /// </summary>
            public string type { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string requestGuid { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string orderId { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string statusCode { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string statusMessage { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public Response response { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string metadata { get; set; }
        }
    }
}