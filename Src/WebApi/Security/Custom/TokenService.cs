using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Domain.Account;
using Mapster.Utils;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Security.Custom
{
    public class TokenService : ITokenService
    {
        public string CreateToken(Func<ITokenBuilder, ITokenBuilder> custom)
        {
            var builder = TokenBuilder.Given()
               .AddAudience(TokenConstants.Audience)
               .AddIssuer(TokenConstants.Issuer)
               .AddExpiry(TokenConstants.ExpiryInMinutes)
               .AddKey(TokenConstants.IssuerSecurityKey)
               .AddEncryptingKey(TokenConstants.EncryptionSecurityKey)
               .AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
            return custom(builder).Build();
        }

        public IPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string x = token.Replace("Bearer ", "", true, null);
            var validationParameters = new TokenValidationParameters()
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = TokenConstants.IssuerSecurityKey,
                ValidIssuer = TokenConstants.Issuer,
                ValidAudience = TokenConstants.Audience,
                ClockSkew = TimeSpan.Zero,
                RoleClaimType = "role",
                NameClaimType = "name",
                TokenDecryptionKey = TokenConstants.EncryptionSecurityKey,
            };
            try
            {
                return tokenHandler.ValidateToken(x, validationParameters, out _);
            }
            catch (Exception)
            {
                IIdentity identity = new GenericIdentity("Invalid", "Invalid");

                return new GenericPrincipal(identity, null);
            }
        }

        public TokenInformations InfosFromToken(string token)
        {
            var principal = GetPrincipal(token) as ClaimsPrincipal;
            var refreshTokenClaim = principal.FindFirst("role");

            var sub = principal.FindFirst(JwtRegisteredClaimNames.Sub);
            var roles = refreshTokenClaim.Value?.Split(",").Select(it => Enum<Roles>.Parse(it)).ToArray();
            return new(sub.Value, roles);
        }

        public string NewRefresToken()
        {
            return Guid.NewGuid().ToString("N");
        }

        public bool ValidateRefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipal(token) as ClaimsPrincipal;
            var refreshTokenClaim = principal.FindFirst(TokenBuilder.RefreshTokenClaim);
            return refreshTokenClaim?.Value == refreshToken;
        }
    }
}

