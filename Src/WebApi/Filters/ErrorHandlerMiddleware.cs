using System;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using OpenTelemetry.Trace;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Filters
{
    namespace GlobalErrorHandling.Extensions
    {
        public static class ExceptionMiddlewareExtensions
        {
            public class ErrorDetails
            {
                public int StatusCode { get; set; }
                public string Message { get; set; }
                public string TraceId { get; set; }

                public override string ToString()
                {
                    return JsonSerializer.Serialize(this);
                }
            }
            public static void UseGlobalExceptionHandler(this IApplicationBuilder app, IServiceProvider serviceProvider)
            {
                var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
                app.UseExceptionHandler(appError =>
                {
                    appError.Run(async context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                        context.Response.ContentType = "application/json";
                        var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                        if (contextFeature != null)
                        {
                            var logger = loggerFactory.CreateLogger("GlobalExceptionHandler");
                            logger.LogError(contextFeature.Error, "Not handled exception.");

                            context.Response.StatusCode = contextFeature.Error switch
                            {
                                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,// not found error
                                _ => (int)HttpStatusCode.InternalServerError,// unhandled error
                            };

                            await context.Response.WriteAsync(new ErrorDetails()
                            {
                                StatusCode = context.Response.StatusCode,
                                Message = contextFeature.Error.Message,
                                TraceId = Tracer.CurrentSpan.Context.TraceId.ToHexString()
                            }.ToString());
                        }
                    });
                });
            }
        }
    }
}

