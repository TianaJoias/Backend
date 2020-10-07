using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using Domain;
using Mapster.Utils;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Security
{
    public class TokenService : ITokenService
    {
        public IPublicTokenBuilder CreateToken()
        {
            return TokenBuilder.Given()
               .AddAudience(TokenConstants.Audience)
               .AddIssuer(TokenConstants.Issuer)
               .AddExpiry(TokenConstants.ExpiryInMinutes)
               .AddKey(TokenConstants.IssuerSecurityKey)
               .AddEncryptingKey(TokenConstants.EncryptionSecurityKey)
               .AddClaim(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        }

        public IPrincipal GetPrincipal(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
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
                return tokenHandler.ValidateToken(token, validationParameters, out _);
            }
            catch (Exception)
            {
                IIdentity identity = new GenericIdentity("Invalid", "Invalid");

                return new GenericPrincipal(identity, null);
            }
        }

        public Roles[] GetRoles(string token)
        {
            var principal = GetPrincipal(token) as ClaimsPrincipal;
            var refreshTokenClaim = principal.FindFirst(ClaimTypes.Role);
            return refreshTokenClaim.Value?.Split(",").Select(it => Enum<Roles>.Parse(it)).ToArray();
        }

        public bool ValidateRefreshToken(string token, string refreshToken)
        {
            var principal = GetPrincipal(token) as ClaimsPrincipal;
            var refreshTokenClaim = principal.FindFirst("RefreshToken");
            return refreshTokenClaim?.Value == refreshToken;
        }
    }
}

