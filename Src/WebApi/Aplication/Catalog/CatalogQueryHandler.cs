using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Domain.Catalog;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Catalog
{
    public class CatalogQueryHandler : IQueryHandler<CatalogQuery, CatalogQueryResult>,
        IQueryHandler<CatalogsByAgentQuery, IList<CatalogsByAgentQueryResult>>
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogQueryHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }
        public async Task<Result<CatalogQueryResult>> Handle(CatalogQuery request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.GetByQuery(it => it.Agent.Id == request.OwnerId && it.Id == request.CatalogId);
            return Result.Ok(catalog.Adapt<CatalogQueryResult>());
        }

        public async Task<Result<IList<CatalogsByAgentQueryResult>>> Handle(CatalogsByAgentQuery request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.List(it => it.Agent.Id == request.OwnerId);
            return Result.Ok(catalog.Adapt<IList<CatalogsByAgentQueryResult>>());
        }
    }

    public class CatalogsByAgentQueryResult
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public decimal TotalSold { get; set; }
    }
    public class CatalogQueryResult : CatalogsByAgentQueryResult
    {
        public IList<CatalogItemQueryResult> Items { get; set; }
    }

    public class CatalogItemQueryResult
    {
        public Guid LotId { get; set; }
        public Guid ProdutoId { get; set; }
        public decimal InitialQuantity { get; set; }
        public decimal CurrentQuantity { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string EAN { get; set; }
        public string LongDescription { get; set; }
        public decimal TotalSold { get; set; }
        public string ShortDescription { get; set; }
        public IList<string> Thumbnail { get; set; }
        public bool Enabled { get; set; }
    }

    public record CatalogsByAgentQuery(Guid OwnerId) : IQuery<IList<CatalogsByAgentQueryResult>>;

    public record CatalogQuery(Guid CatalogId, Guid OwnerId) : IQuery<CatalogQueryResult>;
}
