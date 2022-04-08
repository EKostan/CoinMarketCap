using System;
using System.Net;
using System.Threading.Tasks;
using CoinMarketCap.WebApi.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CoinMarketCap.WebApi.Middleware
{
    public class ErrorHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ErrorHandler> _logger;

        public ErrorHandler(RequestDelegate next, ILogger<ErrorHandler> logger)
        {
            this.next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected
            int errorCode;
            if (exception is AggregateException) code = HttpStatusCode.NotImplemented;
            if (exception is ServiceNotFoundException) code = HttpStatusCode.NotFound;
            if (exception is ApiException apiException)
            {
                errorCode = apiException.ErrorCode;
                code = apiException.HttpCode;
            }
            else
            {
                errorCode = (int)code;
            }

            var message = exception.Message;

            var result = JsonConvert.SerializeObject(new Error { ErrorCode = errorCode, ErrorMessage = message });
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}