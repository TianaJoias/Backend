using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Security
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
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenConstants.EncryptingKey));
                var validationParams = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(TokenConstants.key)),
                    ValidIssuer = TokenConstants.Issuer,
                    ValidAudience = TokenConstants.Audience,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role",
                    NameClaimType = "name",
                    TokenDecryptionKey = securityKey,
                };
                var events = new JwtBearerEvents()
                {
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
                options.Events = events;
                options.TokenValidationParameters = validationParams;
                options.SaveToken = true;
            });
            services.AddAuthorization();
            return services;
        }
    }
}

