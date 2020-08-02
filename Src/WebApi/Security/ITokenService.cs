using System.Collections.Generic;

namespace WebApi.Security
{
    public interface ITokenService
    {
        public string CreateToken(IDictionary<string, string> claims);
    }
}

