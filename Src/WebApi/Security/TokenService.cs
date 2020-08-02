using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace WebApi.Security
{
    public class TokenService : ITokenService
    {
        public string CreateToken(IDictionary<string, string> claimsMap)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N"))
            };
            foreach (var claim in claimsMap)
            {
                claims.Add(new Claim(claim.Key, claim.Value));
            }

            JwtSecurityToken token = new TokenBuilder()
                .AddAudience(TokenConstants.Audience)
                .AddIssuer(TokenConstants.Issuer)
                .AddExpiry(TokenConstants.ExpiryInMinutes)
                .AddKey(TokenConstants.key)
                .AddClaims(claims)
                .Build();

            return new JwtSecurityTokenHandler()
                .WriteToken(token);
        }
    }
}

