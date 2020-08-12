using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
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
        private readonly ITokenService tokenService;

        public AuthenticationController(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        [HttpPost("login"), ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginDTO)), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult Login([FromBody][Required] LoginDTO request)
        {
            if (request.GrantType == GranType.Password)
            {
                if (request.Username.ToUpper().Trim() == "ADMIN" && request.Password == "#PWD123#")
                {
                    var refreshToken = Guid.NewGuid();
                    var token = tokenService.CreateToken()
                        .AddRole(Policies.Admin)
                        .AddSubject($"UserID:{Guid.NewGuid():N}")
                        .AddCustomRole("RefreshToken", refreshToken.ToString("N"))
                        .Build();
                    return Ok(new { token, refreshToken = refreshToken.ToString("N") });
                }
            }
            if (request.GrantType == GranType.RefreshToken)
            {
                var principal = (tokenService.GetPrincipal(request.ExpiredToken) as ClaimsPrincipal);
                var x = principal.Claims.First(it => it.Type == "RefreshToken");
                if(x.Value == request.Token)
                {
                    var refreshToken = Guid.NewGuid();
                    var token = tokenService.CreateToken()
                        .AddRole(Policies.Admin)
                        .AddSubject($"UserID:{Guid.NewGuid():N}")
                        .AddCustomRole("RefreshToken", refreshToken.ToString("N"))
                        .Build();
                    return Ok(new { token, refreshToken = refreshToken.ToString("N") });
                }
                return BadRequest("Refresh Token não implementado." + this.TraceId());
            }
                return BadRequest("Metodo Não implementado." + this.TraceId());
        }
    }
}
