using System.Collections.Generic;
using System.Security.Principal;

namespace WebApi.Security
{
    public interface ITokenService
    {
        public IPublicTokenBuilder CreateToken();
        public IPrincipal GetPrincipal(string token);
    }
}

