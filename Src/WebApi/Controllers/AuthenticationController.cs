using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security;

namespace WebApi.Controllers
{
    public static class TraceIdExtensions
    {
        public static string TraceId(this ControllerBase controller)
        {
            return Activity.Current?.Id ?? controller.HttpContext?.TraceIdentifier;
        }
    }

    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly IAccountRepository _accountRepository;

        public AuthenticationController(ITokenService tokenService, IAccountRepository accountRepository)
        {
            _tokenService = tokenService;
            _accountRepository = accountRepository;
        }

        [HttpPost("login"), ProducesResponseType(StatusCodes.Status200OK), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody][Required] LoginDTO request)
        {
            if (request.GrantType == GranType.Password)
            {
                var user = await _accountRepository.GetByQuery(it => it.User.Email.ToLower() == request.Username.ToLower());
                if (user is not null && BCrypt.Net.BCrypt.Verify(request.Password, user.User.Password))
                {
                    var (token, refreshToken) = CreateToken(user.Roles.ToArray());
                    return Ok(new { token, refreshToken });
                }
                return BadRequest("Password or Username not match.");
            }

            if (request.GrantType == GranType.RefreshToken)
            {
                if (_tokenService.ValidateRefreshToken(request.ExpiredToken, request.Token))
                {
                    var roles = _tokenService.GetRoles(request.ExpiredToken);
                    var (token, refreshToken) = CreateToken(roles);
                    return Ok(new { token, refreshToken });
                }
                return BadRequest("Refresh Token invalido." + this.TraceId());
            }

            return BadRequest("Metodo Não implementado." + this.TraceId());
        }


        private (string token, string refreshToken) CreateToken(params Roles[] roles)
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var userId = Guid.NewGuid().ToString();
            var token = _tokenService.CreateToken()
                .AddRoles(roles.Select(it => it.ToString("G")).ToArray())
                .AddSubject(userId)
                .AddRefreshToken(refreshToken)
                .Build();
            return (token, refreshToken);
        }
    }
}
