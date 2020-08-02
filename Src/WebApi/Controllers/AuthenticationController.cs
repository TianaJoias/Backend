using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Security;

namespace WebApi.Controllers
{
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
        public async Task<IActionResult> Login([FromBody][Required] LoginDTO request)
        {
            if (request.Username.ToUpper().Trim() == "ADMIN" && request.Password == "#PWD123#")
            {
                var claims = new Dictionary<string, string> { { ClaimTypes.Role, Policies.Admin },
                    { JwtRegisteredClaimNames.Sub, $"UserID:{Guid.NewGuid():N}"}
                };
                var token = tokenService.CreateToken(claims);
                return Ok(new { token });
            }
            return BadRequest("Login ou Senha não encontrados.");
        }
    }
}
