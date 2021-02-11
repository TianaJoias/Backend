using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Aplication;
using WebApi.Aplication.Catalog.Commands;
using WebApi.Aplication.Catalog.Queries;
using static Domain.Catalog.Catalog;

namespace WebApi.Controllers
{
    [Route("v{version:apiVersion}/[controller]")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AgentsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] FilterPaged request)
        {
            var query = request.Adapt<AgentsQuery>();
            query.AccountableId = User.GetId();
            var result = await _mediator.Send(query);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] AgentRequest request)
        {
            var createAgentCommand = new CreateAgentCommand(request.Name, request.Email, User.GetId()
            );
            var result = await _mediator.Send(createAgentCommand);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/catalogs")]
        public async Task<IActionResult> Catalog(Guid id, [FromBody] CatalogAddPackageRequest request)
        {
            var command = new CatalogAddPackageCommand(id, User.GetId(), request.Items, request.Done);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/catalogs/{catalogId:guid}/status")]
        public async Task<IActionResult> NextState(Guid id, Guid catalogId, [FromBody] string comment)
        {
            var command = new CatalogNextStatusCommand(catalogId, User.GetId(), id, comment);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/catalogs/{catalogId:guid}/close")]
        public async Task<IActionResult> CatalogPut(Guid id, Guid catalogId, [FromBody] CatalogClosePackageRequest request)
        {
            var command = new CatalogClosePackageCommand
            {
                OwnerId = id,
                CatalogId = catalogId,
                Done = request.Done,
                Items = request.Items
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPost("{id:guid}/catalogs/{catalogId:guid}/transfer")]
        public async Task<IActionResult> transfer(Guid id, Guid catalogId, [FromBody] CatalogItemTransferPackageRequest request)
        {
            var command = new CatalogTransferItemsCommand
            {
                AgentId = id,
                FromCatalogId = catalogId,
                ToCatalogId = request.ToCatalogId,
                Items = request.Items
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPut("me/catalogs/{catalogId:guid}")]
        public Task<IActionResult> CatalogPut(Guid catalogId, [FromBody] CatalogClosePackageRequest request)
        {
            return CatalogPut(User.GetId(), catalogId, request);
        }

        [HttpGet("me/catalogs/{id:guid}")]
        public async Task<IActionResult> CatalogGet(Guid id)
        {
            var command = new CatalogQuery(id, User.GetId());
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("me/catalogs")]
        public async Task<IActionResult> Catalogs([FromQuery] CatalogQueryString catalogQueryString)
        {
            var command = catalogQueryString.Adapt<CatalogsByAgentQuery>();
            command.OwnerId = User.GetId();
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}/catalogs")]
        public async Task<IActionResult> Catalogs(Guid id, [FromQuery] CatalogQueryString catalogQueryString)
        {
            var command = catalogQueryString.Adapt<CatalogsByAgentQuery>();
            command.OwnerId = id;
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }


        [HttpGet("{agentId:guid}/catalogs/{catalogId:guid}")]
        public async Task<IActionResult> CatalogGet(Guid agentId, Guid catalogId)
        {
            var command = new CatalogQuery(catalogId, agentId);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }
    }

    public class CatalogQueryString : FilterPaged
    {
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<States> States { get; set; }
    }

    public record CatalogItemTransferPackageRequest
    {
        public IList<CatalogClosePackageItemCommand> Items { get; init; }
        public Guid ToCatalogId { get; set; }
    }

    public record CatalogClosePackageRequest
    {
        public IList<CatalogClosePackageItemCommand> Items { get; init; }
        public bool Done { get; set; }
    }

    public record CatalogAddPackageRequest
    {
        public IList<CatalogAddPackageItemCommand> Items { get; init; }
        public bool Done { get; set; }
    }


    public record AgentRequest
    {
        public string Email { get; init; }
        public string Name { get; init; }
    }
}
