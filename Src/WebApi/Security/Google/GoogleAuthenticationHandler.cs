using Domain;
using Domain.Account;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace WebApi.Security.Google
{
    public class GoogleAuthenticationHandler : AuthenticationHandler<GoogleAuthOptions>
    {
        private readonly IAccountRepository _accountRepository;

        public GoogleAuthenticationHandler(
            IOptionsMonitor<GoogleAuthOptions> options,
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
                return AuthenticateResult.NoResult();

            if (!authorizationHeader.StartsWith(GoogleAuthenticationDefaults.AuthenticationScheme, StringComparison.OrdinalIgnoreCase))
                return AuthenticateResult.Fail("Unauthorized");

            string token = authorizationHeader[GoogleAuthenticationDefaults.AuthenticationScheme.Length..].Trim();
            if (string.IsNullOrEmpty(token))
                return AuthenticateResult.Fail("Unauthorized");

            return await ValidateTokenAsync(token);
        }

        private async Task<AuthenticateResult> ValidateTokenAsync(string securityToken)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(securityToken);
                var account = await _accountRepository.GetByQuery(it => it.ExternalProviders.Any(x => x.SubjectId == payload.Subject && x.Provider == GoogleAuthenticationDefaults.AuthenticationScheme));
                if (account is not null)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, payload.Name),
                        new Claim(ClaimTypes.Name, payload.Name),
                        new Claim(JwtRegisteredClaimNames.FamilyName, payload.FamilyName),
                        new Claim(JwtRegisteredClaimNames.GivenName, payload.GivenName),
                        new Claim(JwtRegisteredClaimNames.Email, payload.Email),
                        new Claim(JwtRegisteredClaimNames.Sub, account.Id.ToString()),
                        new Claim(JwtRegisteredClaimNames.Iss, payload.Issuer),
                    };
                    var roles = account.Roles.Select(it => new Claim(ClaimTypes.Role, it.ToString("G"))).ToList();
                    claims.AddRange(roles);
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

