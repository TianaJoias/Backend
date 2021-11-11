using System;
using Application;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace WebApi
{
    public class WebAppContext : IAppContext
    {
        private readonly IHttpContextAccessor _httpContext;

        public WebAppContext(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext ?? throw new ArgumentNullException(nameof(httpContext));
        }
        public Task<string> GetCurrentContext()
        {
           return Task.Run(()=> _httpContext.HttpContext.TraceIdentifier);
        }
    }
}
