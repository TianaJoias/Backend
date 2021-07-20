using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Application.Account;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        private readonly IMediator _mediator;

        public AuthenticationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login"), ProducesResponseType(StatusCodes.Status200OK), AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> LoginAsync([FromBody][Required] LoginRequest request)
        {
            if (request.GrantType == GranType.Password)
            {
                var result = await _mediator.Send(new PasswordLoginQuery(request.Username, request.Password));
                return result.ToActionResult();
            }

            if (request.GrantType == GranType.RefreshToken)
            {
                var result = await _mediator.Send(new RefreshLoginQuery(request.Token, request.ExpiredToken));
                return result.ToActionResult();
            }

            return BadRequest("Metodo Não implementado." + this.TraceId());
        }
    }
    public class ErrorResponse
    {
        public string Message { get; set; }

        public string Code { get; set; }

        public ErrorResponse(string message, string code)
        {
            Message = message;
            Code = code;
        }
    }

    public static class ResultExtensions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            return new BadRequestObjectResult(TransformErrors(result.Errors));
        }
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            return new BadRequestObjectResult(TransformErrors(result.Errors));
        }
        private static IEnumerable<ErrorResponse> TransformErrors(List<Error> errors)
        {
            return errors.Select(TransformError);
        }

        private static ErrorResponse TransformError(Error error)
        {
            var errorCode = TransformErrorCode(error);

            return new ErrorResponse(error.Message, errorCode);
        }

        private static string TransformErrorCode(Error error)
        {
            if (error.Metadata.TryGetValue("ErrorCode", out var errorCode))
                return errorCode as string;

            return "";
        }
    }
}
