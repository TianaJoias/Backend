using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace WebApi.Security.Custom
{
    public static class CustomAuthenticationExtensions
    {
        public static AuthenticationBuilder AddCustomJWTAuthentication(this AuthenticationBuilder services)
        {
            return services.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
            {
                var validationParams = new TokenValidationParameters()
                {
                    ValidateAudience = true,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = TokenConstants.IssuerSecurityKey,
                    ValidIssuer = TokenConstants.Issuer,
                    ValidAudience = TokenConstants.Audience,
                    ClockSkew = TimeSpan.Zero,
                    RoleClaimType = "role",
                    NameClaimType = "name",
                    TokenDecryptionKey = TokenConstants.EncryptionSecurityKey,
                };
                var events = new JwtBearerEvents()
                {
                    OnAuthenticationFailed = (context) =>
                    {
                        return Task.CompletedTask;
                    },
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
        }

        public static AuthenticationBuilder AddAuthenticationSmart(this IServiceCollection services, string defaultSchema = JwtBearerDefaults.AuthenticationScheme, (string Key, string Value)[] strategies = default)
        {
            return services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "smart";
                options.DefaultChallengeScheme = "smart";
            })
              .AddPolicyScheme("smart", "Bearer or Secret Header", options =>
              {
                  options.ForwardDefaultSelector = context =>
                  {
                      var authHeader = context.Request.Headers["Authorization"].ToString();
                      if (string.IsNullOrWhiteSpace(authHeader))
                          return defaultSchema;
                      strategies ??= Array.Empty<(string Key, string Value)>();
                      foreach (var (Key, Value) in strategies)
                      {
                          if (authHeader.StartsWith(Key))
                              return Value;
                      }

                      return defaultSchema;
                  };
              });
        }
    }
}

