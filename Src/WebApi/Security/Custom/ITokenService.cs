using System;
using Domain.Account;

namespace WebApi.Security.Custom
{
    public interface ITokenService
    {
        public string CreateToken(Func<ITokenBuilder, ITokenBuilder> custom);
        public string NewRefresToken();
        public bool ValidateRefreshToken(string token, string refreshToken);
        public TokenInformations InfosFromToken(string token);
    }

    public record TokenInformations(string Subject, Roles[] Roles);
    public interface ITokenBuilder
    {
        ITokenBuilder AddSubject(string subject);
        ITokenBuilder AddRole(string role);
        ITokenBuilder AddRoles(string[] roles);
        ITokenBuilder AddRefreshToken(string refreshToken);
        ITokenBuilder AddCustomRole(string roleName, string roleValue);
        string Build();
    }
}

