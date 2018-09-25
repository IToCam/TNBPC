using System;
using System.Net;

namespace BPC.Authenticate
{
    [Serializable]
    public class MontelTicket
    {
        public string Token { get; set; }
        public string Status { get; set; }
        public Exception InnerException { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public DateTime ExpireTime { get; set; }
    }
}
