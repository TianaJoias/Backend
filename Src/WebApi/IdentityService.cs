using System;
using Application;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace WebApi
{
    public class IdentityService : IIdentityService
    {
        private readonly IHttpContextAccessor _context;

        public IdentityService(IHttpContextAccessor context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public Guid GetUserIdentity()
        {
            var userId = _context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return new Guid(userId);
        }

        public string GetUserEmail()
        {
            var userEmail = _context.HttpContext.User.FindFirst("emails").Value;
            return userEmail;
        }
    }
}
