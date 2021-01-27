using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using FluentResults;
using Infra;
using Mapster;
using static Domain.Catalog.Catalog;

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
            Expression<Func<Domain.Catalog.Catalog, bool>> baseWhere = it => it.Agent.Id == request.OwnerId;

            if (request.States is not null && request.States.Any())
                baseWhere = baseWhere.And(it => request.States.Contains(it.State));
            if (request.FromDate is not null)
                baseWhere = baseWhere.And(it => it.CreatedAt >= request.FromDate.Value.Date);
            if (request.ToDate is not null)
                baseWhere = baseWhere.And(it => it.CreatedAt <= request.ToDate.Value.Date.AddDays(1));
            var catalog = await _catalogRepository.GetPaged(baseWhere, request.Page, request.PageSize, request.OrderBy);
            return Result.Ok(catalog.Records.Adapt<IList<CatalogsByAgentQueryResult>>());
        }
    }

    public class CatalogsByAgentQueryResult
    {
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public States State { get; set; }
        public decimal SoldValue { get; set; }
        public int ItemsQuantity { get; set; }
        public decimal ValuedAt { get; set; }
        public decimal ItemsAddedQuantity { get; set; }
        public decimal SoldQuantity { get; set; }
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

    public abstract class BaseQuery<T> : IQuery<T>
    {
        public string Term { get; set; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 5;
        public Dictionary<string, Sort> OrderBy { get; init; } = new Dictionary<string, Sort> { { "CreatedAt", Sort.Desc } };
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IList<States> States { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class CatalogsByAgentQuery : BaseQuery<IList<CatalogsByAgentQueryResult>>
    {

    }

    public record CatalogQuery(Guid CatalogId, Guid OwnerId) : PaginationQuery, IQuery<CatalogQueryResult>;
}
