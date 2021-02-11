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

namespace WebApi.Aplication.Catalog.Queries
{
    public class CatalogQueryHandler : IQueryHandler<CatalogQuery, CatalogWithItemsDTO>,
        IQueryHandler<CatalogsByAgentQuery, PagedData<CatalogDTO>>
    {
        private readonly ICatalogRepository _catalogRepository;

        public CatalogQueryHandler(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }
        public async Task<Result<CatalogWithItemsDTO>> Handle(CatalogQuery request, CancellationToken cancellationToken)
        {
            var catalog = await _catalogRepository.GetByQuery(it => it.Agent.Id == request.OwnerId && it.Id == request.CatalogId);
            return Result.Ok(catalog.Adapt<CatalogWithItemsDTO>());
        }

        public async Task<Result<PagedData<CatalogDTO>>> Handle(CatalogsByAgentQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Domain.Catalog.Catalog, bool>> baseWhere = it => it.Agent.Id == request.OwnerId;

            if (request.States is not null && request.States.Any())
                baseWhere = baseWhere.And(it => request.States.Contains(it.State));
            if (request.FromDate is not null)
                baseWhere = baseWhere.And(it => it.CreatedAt >= request.FromDate.Value.Date);
            if (request.ToDate is not null)
                baseWhere = baseWhere.And(it => it.CreatedAt <= request.ToDate.Value.AddDays(1).Date.AddSeconds(-1));
            var catalog = await _catalogRepository.GetPaged(baseWhere, request.Page, request.PageSize, request.OrderBy);
            return Result.Ok(catalog.Adapt<PagedData<CatalogDTO>>());
        }
    }

    public class CatalogDTO
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
    public class CatalogWithItemsDTO : CatalogDTO
    {
        public IList<CatalogItemDTO> Items { get; set; }
    }

    public class CatalogItemDTO
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


    public class CatalogsByAgentQuery : FilterPagedQuery<PagedData<CatalogDTO>>
    {
        public CatalogsByAgentQuery() : base(("CreatedAt", Sort.Desc))
        {

        }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public IList<States> States { get; set; }
        public Guid OwnerId { get; set; }
    }

    public class CatalogQuery : IQuery<CatalogWithItemsDTO>
    {
        public CatalogQuery(Guid catalogId, Guid ownerId)
        {
            CatalogId = catalogId;
            OwnerId = ownerId;
        }

        public Guid CatalogId { get; }
        public Guid OwnerId { get; }
    }
}
