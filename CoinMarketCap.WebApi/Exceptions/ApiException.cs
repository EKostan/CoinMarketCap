using System;
using System.Net;

namespace CoinMarketCap.WebApi.Exceptions
{
    public class ApiException : Exception
    {
        public int ErrorCode { get; set; }
        public HttpStatusCode HttpCode { get; set; }

        public ApiException(int errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
            HttpCode = GetHttpCode(errorCode);
        }

        public ApiException(int errorCode, string message, HttpStatusCode httpCode) : base(message)
        {
            ErrorCode = errorCode;
            HttpCode = httpCode;
        }

        private HttpStatusCode GetHttpCode(int errorCode)
        {
            var cat = errorCode / 1000;
            switch (cat)
            {
                case 1: return HttpStatusCode.BadRequest;
                case 2: return HttpStatusCode.InternalServerError;
                case 3: return HttpStatusCode.InternalServerError;
                case 4: return HttpStatusCode.InternalServerError;
            }
            return HttpStatusCode.InternalServerError;
        }
    }
}