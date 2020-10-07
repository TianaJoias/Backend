using Domain;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

using System.Text.Encodings.Web;
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
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = "smart";
            //    options.DefaultChallengeScheme = "smart";
            //})
            //    .AddPolicyScheme("smart", "Bearer or Secret Header", options =>
            //{
            //    options.ForwardDefaultSelector = context =>
            //    {
            //        var authHeader = context.Request.Headers["Authorization"].ToString();
            //        if (authHeader?.StartsWith(GoogleAuthenticationDefaults.AuthenticationScheme) == true)
            //            return GoogleAuthenticationDefaults.AuthenticationScheme;

            //        return JwtBearerDefaults.AuthenticationScheme;
            //    };
            //})
            services.AddAuthenticationSmart(strategies: new[] { (GoogleAuthenticationDefaults.AuthenticationScheme, GoogleAuthenticationDefaults.AuthenticationScheme), })
                .AddGoogleAuthentication()
               .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
             {
                 // var provider = services.BuildServiceProvider();
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
            services.AddAuthorization(options =>
                 {
                     var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                          JwtBearerDefaults.AuthenticationScheme,
                          GoogleDefaults.AuthenticationScheme);
                     defaultAuthorizationPolicyBuilder =
                         defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser();

                     options.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();
                 });
            return services;
        }
    }

    public static class AddGoogleAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGoogleAuthentication(this AuthenticationBuilder services)
        {
            services.AddScheme<CustomAuthOptions, GoogleAuthenticationHandler>(GoogleAuthenticationDefaults.AuthenticationScheme, o => o.UserInfoEndpoint = "");

            return services;
        }
        public static AuthenticationBuilder AddAuthenticationSmart(this IServiceCollection services, string defaultSchema = JwtBearerDefaults.AuthenticationScheme, (string Key, string Value)[] strategies = null)
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
                      strategies ??= Array.Empty<(string Key, string Value)>();
                      foreach (var (Key, Value) in strategies)
                      {
                          if (authHeader?.StartsWith(Key) == true)
                              return Value;
                      }

                      return defaultSchema;
                  };
              });
        }
    }
    public static class GoogleAuthenticationDefaults
    {
        public const string AuthenticationScheme = GoogleDefaults.AuthenticationScheme;
    }
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public string UserInfoEndpoint { get; set; }
    }
    public class GoogleAuthenticationHandler : AuthenticationHandler<CustomAuthOptions>
    {
        private readonly IAccountRepository _accountRepository;

        public GoogleAuthenticationHandler(
            IOptionsMonitor<CustomAuthOptions> options,
            ILoggerFactory logger,
            IAccountRepository accountRepository,
            UrlEncoder encoder,
            ISystemClock clock) : base(options, logger, encoder, clock)
        {
            _accountRepository = accountRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return AuthenticateResult.Fail("Unauthorized");

            string authorizationHeader = Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authorizationHeader))
            {
                return AuthenticateResult.NoResult();
            }
            if (!authorizationHeader.StartsWith(GoogleAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }

            string token = authorizationHeader.Substring(GoogleAuthenticationDefaults.AuthenticationScheme.Length).Trim();
            if (string.IsNullOrEmpty(token))
            {
                return AuthenticateResult.Fail("Unauthorized");
            }
            try
            {
                return await ValidateTokenAsync(token);
            }
            catch (Exception ex)
            {
                return AuthenticateResult.Fail(ex.Message);
            }
        }

        private async Task<AuthenticateResult> ValidateTokenAsync(string securityToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(securityToken);
                var account = (await _accountRepository.List(it => it.ExternalProviders.Any(x => x.SubjectId == payload.Subject && x.Provider == GoogleAuthenticationDefaults.AuthenticationScheme))).FirstOrDefault();
                if (account is not null)
                {
                    var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, payload.Name),
                    new Claim(ClaimTypes.Name, payload.Name),
                    new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                    new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                    new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, payload.Subject),
                    new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
                };

                    var identity = new ClaimsIdentity(claims, Scheme.Name);
                    var principal = new ClaimsPrincipal(identity);
                    var ticket = new AuthenticationTicket(principal, Scheme.Name);
                    return AuthenticateResult.Success(ticket);
                }
                return AuthenticateResult.Fail("UserNotFound");
            }
            catch (Exception e)
            {
                return AuthenticateResult.Fail(e.Message);

            }
        }
    }
}

