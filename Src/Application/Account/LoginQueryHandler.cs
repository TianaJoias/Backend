using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain.Account;
using FluentResults;

namespace Application.Account
{
    public class LoginQueryHandler : IQueryHandler<PasswordLoginQuery, LoginQueryResult>,
        IQueryHandler<RefreshLoginQuery, LoginQueryResult>
    {
        private readonly ITokenService _tokenService;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordService _passwordService;

        public LoginQueryHandler(ITokenService tokenService, IAccountRepository accountRepository, IPasswordService passwordService)
        {
            _tokenService = tokenService;
            _accountRepository = accountRepository;
            _passwordService = passwordService;
        }

        public async Task<Result<LoginQueryResult>> Handle(PasswordLoginQuery request, CancellationToken cancellationToken)
        {
            var account = await _accountRepository.Find(it => it.User.Email.ToLower() == request.Username.ToLower());
            if (account is not null && await _passwordService.Verify(request.Password, account?.User.Password))
            {
                var result = CreateToken(account.Id.ToString(), account.Roles.ToArray());
                return Result.Ok(result);
            }
            return Result.Fail("Password or Username not match.");
        }

        public Task<Result<LoginQueryResult>> Handle(RefreshLoginQuery request, CancellationToken cancellationToken)
        {
            if (_tokenService.ValidateRefreshToken(request.ExpiredToken, request.RefreshToken))
            {
                var infos = _tokenService.InfosFromToken(request.ExpiredToken);
                var result = CreateToken(infos.Subject, infos.Roles);
                return Task.FromResult(Result.Ok(result));
            }
            return Task.FromResult<Result<LoginQueryResult>>(Result.Fail("Token ou Refresh Token invalido."));
        }

        private LoginQueryResult CreateToken(string userId, params Roles[] roles)
        {
            var refreshToken = _tokenService.NewRefresToken();
            var token = _tokenService.CreateToken(builder => builder
                .AddRoles(roles.Select(it => it.ToString("G")).ToArray())
                .AddSubject(userId)
                .AddRefreshToken(refreshToken));
            return new(token, refreshToken);
        }
    }

    public record LoginQueryResult(string Token, string RefreshToken);

    public record PasswordLoginQuery : IQuery<LoginQueryResult>
    {
        public string Password { get; init; }
        public string Username { get; init; }
        public PasswordLoginQuery(string username, string password)
            => (Username, Password) = (username, password);
    }

    public record RefreshLoginQuery : IQuery<LoginQueryResult>
    {
        public string RefreshToken { get; init; }
        public string ExpiredToken { get; init; }
        public RefreshLoginQuery(string refreshToken, string expiredToken)
            => (RefreshToken, ExpiredToken) = (refreshToken, expiredToken);
    }
}
