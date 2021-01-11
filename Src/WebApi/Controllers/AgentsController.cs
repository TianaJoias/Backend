using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Aplication.Catalog;

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
        public async Task<IActionResult> Get([FromQuery] PaginateRequest request)
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
            var items = request.Items.Adapt<IList<CatalogOpenItemCommand>>();
            var command = new CatalogAddPackageCommand(id, User.GetId(), items, request.Done);
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPut("{id:guid}/catalogs/{catalogId:guid}")]
        public async Task<IActionResult> CatalogPut(Guid id, Guid catalogId, [FromBody] IList<ItemCatalogCloseRequest> items)
        {
            var command = new CatalogClosePackageCommand
            {
                OwnerId = id,
                CatalogId = catalogId,
                Items = items.Adapt<IList<CatalogCloseItemCommand>>()
            };
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpPut("me/catalogs/{catalogId:guid}")]
        public Task<IActionResult> CatalogPut(Guid catalogId, [FromBody] IList<ItemCatalogCloseRequest> items)
        {
            return CatalogPut(User.GetId(), catalogId, items);
        }

        [HttpGet("me/catalogs/{id:guid}")]
        public async Task<IActionResult> CatalogGet(Guid id)
        {
            var command = new CatalogQuery(id, User.GetId());
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("me/catalogs")]
        public async Task<IActionResult> Catalogs()
        {
            var command = new CatalogsByAgentQuery(User.GetId());
            var result = await _mediator.Send(command);
            return result.ToActionResult();
        }

        [HttpGet("{id:guid}/catalogs")]
        public async Task<IActionResult> Catalogs(Guid id)
        {
            var command = new CatalogsByAgentQuery(id);
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

    public record CatalogAddPackageRequest
    {
        public IList<CatalogOpenItemRequest> Items { get; init; }
        public bool Done { get; set; }
    }

    public record CatalogOpenItemRequest
    {
        public Guid LotId { get; init; }
        public decimal Quantity { get; init; }
    }

    public record ItemCatalogCloseRequest
    {
        public Guid LotId { get; set; }
        public decimal Quantity { get; set; }
    }

    public record PaginateRequest
    {
        public string SearchTerm { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 5;
        public Dictionary<string, Sort> OrderBy { get; init; } = null;
    }

    public record AgentRequest
    {
        public string Email { get; init; }
        public string Name { get; init; }
    }
}
