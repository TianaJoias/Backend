using System.Collections.Generic;
using System.Security.Principal;
using Domain;

namespace WebApi.Security
{
    public interface ITokenService
    {
        public IPublicTokenBuilder CreateToken();
        public IPrincipal GetPrincipal(string token);
        public bool ValidateRefreshToken(string token, string refreshToken);
        public Roles[] GetRoles(string token);
    }
}

