using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
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

        public AuthenticationController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login"), ProducesResponseType(StatusCodes.Status200OK), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody][Required] LoginDTO request)
        {
            if (request.GrantType == GranType.Password)
            {
                    var password = BCrypt.Net.BCrypt.EnhancedHashPassword("ADMIN");
                if (request.Username.ToUpper().Trim() == "ADMIN" && request.Password == "ADMIN")
                {
                    var (token, refreshToken) = CreateToken();
                    return Ok(new { token, refreshToken });
                }

                return BadRequest("Password or Username not match.");
            }

            if (request.GrantType == GranType.RefreshToken)
            {
                if (RefreshTokenIsValid(request))
                {
                    var (token, refreshToken) = CreateToken();
                    return Ok(new { token, refreshToken });
                }
                return BadRequest("Refresh Token não pertence a esse token." + this.TraceId());
            }

            return BadRequest("Metodo Não implementado." + this.TraceId());
        }

        private bool RefreshTokenIsValid(LoginDTO request)
        {
            var principal = _tokenService.GetPrincipal(request.ExpiredToken) as ClaimsPrincipal;
            var refreshTokenClaim = principal?.FindFirst("RefreshToken");
            return refreshTokenClaim?.Value == request.Token;
        }

        private (string token, string refreshToken) CreateToken()
        {
            var refreshToken = Guid.NewGuid().ToString("N");
            var userId = Guid.NewGuid().ToString();
            var token = _tokenService.CreateToken()
                .AddRole(Policies.Admin)
                .AddSubject(userId)
                .AddCustomRole("RefreshToken", refreshToken)
                .Build();
            return (token, refreshToken);
        }
    }
}
