using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Api.Extensions
{
    //https://fullstackmark.com/post/19/jwt-authentication-flow-with-refresh-tokens-in-aspnet-core-web-api
    public static class SecurityExtensions
    {
        public static IServiceCollection AddSecurity(this IServiceCollection services)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddScoped<ITokenService, TokenService>();
            services.AddAuthentication(authOptions =>
            {
                authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                var provider = services.BuildServiceProvider();
                var validationParams = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenConstants.key)),
                    ValidIssuer = TokenConstants.Issuer,
                    ValidAudience = TokenConstants.Audience,
                    ClockSkew = TimeSpan.Zero
                };
                var events = new JwtBearerEvents()
                {
                    // invoked when the token validation fails
                    OnAuthenticationFailed = (context) =>
                    {
                        Console.WriteLine(context.Exception);
                        return Task.CompletedTask;
                    },

                    // invoked when a request is received
                    OnMessageReceived = (context) =>
                    {
                        return Task.CompletedTask;
                    },

                    // invoked when token is validated
                    OnTokenValidated = (context) =>
                    {
                        //var security = arg.HttpContext.RequestServices.GetService<ISecurityService>();
                        //if (await security.IsRevoked(arg.Principal))
                        //{
                        //    // it`s blacklisted!
                        //    // there's a bunch of built-in token validation codes: https://github.com/AzureAD/azure-activedirectory-identitymodel-extensions-for-dotnet/blob/7692d12e49a947f68a44cd3abc040d0c241376e6/src/Microsoft.IdentityModel.Tokens/LogMessages.cs
                        //    // but none of them is suitable for this
                        //    arg.Fail(new SecurityTokenRevokedException($"The token has been revoked, securitytoken: '{arg.SecurityToken}'."));
                        //}
                        return Task.CompletedTask;
                    }
                };
                //var fileprovider = provider.GetService<IFileProvider>();
                //var file = fileprovider.GetFileInfo(signingConfigurations.PfxFile);
                options.Events = events;
                options.TokenValidationParameters = validationParams;
                options.SaveToken = true;
            });
            services.AddAuthorization(config =>
            {
                config.AddPolicy("ShouldContainRole",
                    options => options.RequireClaim(ClaimTypes.Role));
            });
            return services;
        }
    }

    public class TokenConstants
    {
        public static string Issuer = "thisismeyouknow";
        public static string Audience = "thisismeyouknow";
        public static int ExpiryInMinutes = 1000;
        public static string key = "thiskeyisverylargetobreak";
    }

    public class Policies
    {
        public const string Admin = "Admin";
        public const string User = "User";
        public static AuthorizationPolicy AdminPolicy() { return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Admin).Build(); }
        public static AuthorizationPolicy UserPolicy() { return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(User).Build(); }
    }
    public interface ITokenService
    {
        public string CreateToken(IDictionary<string, string> claims);
    }

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
    public class TokenBuilder
    {
        private string _issuer;
        private string _audience;
        private DateTime _expires;
        private SigningCredentials _credentials;
        private SymmetricSecurityKey _key;
        private List<Claim> _claims;

        public TokenBuilder AddClaims(List<Claim> claims)
        {
            if (_claims == null)
                _claims = claims;
            else
                _claims.AddRange(claims);
            return this;
        }

        public TokenBuilder AddClaim(Claim claim)
        {
            if (_claims == null)
                _claims = new List<Claim>() { claim };
            else
                _claims.Add(claim);
            return this;
        }

        public TokenBuilder AddIssuer(string issuer)
        {
            _issuer = issuer;
            return this;
        }

        public TokenBuilder AddAudience(string audience)
        {
            _audience = audience;
            return this;
        }

        public TokenBuilder AddExpiry(int minutes)
        {
            _expires = DateTime.Now.AddMinutes(minutes);
            return this;
        }

        public TokenBuilder AddKey(string key)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            _credentials = new SigningCredentials(_key,
            SecurityAlgorithms.HmacSha256);
            return this;
        }

        public JwtSecurityToken Build()
        {
            return new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: _claims,
                expires: _expires,
                signingCredentials: _credentials
            );
        }
    }
}

