using System;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace WebApi.Filters
{
    public class ErrorHandlerMiddleware: IMiddleware
    {
        private readonly ILogger<ErrorHandlerMiddleware> _logger;

        public ErrorHandlerMiddleware(ILogger<ErrorHandlerMiddleware> logger)
        {
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            try
            {
                await next(context);
            }
            catch (Exception error)
            {
                _logger.LogError(error, "Not handled exception.");
                var response = context.Response;
                response.ContentType = "application/json";
                response.StatusCode = error switch
                {
                    UnauthorizedAccessException => StatusCodes.Status401Unauthorized,// not found error
                    _ => (int)HttpStatusCode.InternalServerError,// unhandled error
                };
                var result = JsonSerializer.Serialize(new { message = error?.Message });
                await response.WriteAsync(result);
            }
        }
    }
}

