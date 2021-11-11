using Application.Products.Commands;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace WebApi.Controllers
{

    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(POLICIES.ADMIN)]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateProductCommand createProductCommand, [FromHeader(Name = "x-requestid")] string requestId)
        {
            Result commandResult = default;

            if (Guid.TryParse(requestId, out Guid guid) && guid != Guid.Empty)
            {
                createProductCommand.Id = guid;
                commandResult = await _mediator.Send(createProductCommand);
            }

            if (!commandResult.IsFailed)
            {
                return BadRequest();
            }

            return Ok(commandResult.IsSuccess);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ProductQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result.Value);
        }
    }
}
