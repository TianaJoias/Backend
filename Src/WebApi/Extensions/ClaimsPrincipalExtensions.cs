using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetId(this ClaimsPrincipal user)
        {
            var value = user.FindFirstValue(JwtRegisteredClaimNames.Sub);
            if (value is null)
                throw new UnauthorizedAccessException("User Not Found.");
            return Guid.Parse(value);
        }
    }

}
