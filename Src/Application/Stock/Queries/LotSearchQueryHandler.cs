using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock.Queries
{
    public class LotSearchQueryHandler : IQueryHandler<LotSearchQuery, LotResult>, IQueryHandler<LotsByProductIdQuery, PagedData<LotResponse>>
    {
        private readonly ILotRepository _lotRepository;
        private readonly IProductRepository _productRepository;

        public LotSearchQueryHandler(ILotRepository lotRepository, IProductRepository productRepository)
        {
            _lotRepository = lotRepository;
            _productRepository = productRepository;
        }

        public async Task<Result<LotResult>> Handle(LotSearchQuery request, CancellationToken cancellationToken)
        {
            var lot = await _lotRepository.GetByQuery(it => it.EAN.Equals(request.ean));
            if (lot is null)
                return Result.Ok<LotResult>(null);
            var product = await _productRepository.GetById(lot.ProductId);
            var lotResult = product.Adapt<LotResult>();
            lotResult = lot.Adapt(lotResult);
            return Result.Ok(lotResult);
        }

        public async Task<Result<PagedData<LotResponse>>> Handle(LotsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var lots = await _lotRepository.GetPaged(it => it.ProductId == request.ProductId, request.Page, request.PageSize, request.OrderBy);
            var lotResult = lots.Adapt<PagedData<LotResponse>>();
            return Result.Ok(lotResult);
        }
    }

    public class LotsByProductIdQuery : FilterPagedQuery<PagedData<LotResponse>>
    {
        public Guid ProductId { get; set; }
    }

    public record LotSearchQuery(string ean) : IQuery<LotResult>;

    public record LotResult
    {
        public Guid Id { get; set; }
        public string Number { get; init; }
        public Guid ProductId { get; init; }
        public string SKU { get; init; }
        public string Description { get; init; }
        public decimal CostPrice { get; init; }
        public string EAN { get; init; }
    }

    public record LotResponse
    {
        public Guid Id { get; init; }
        public Guid ProductId { get; init; }
        public decimal CostPrice { get; init; }
        public decimal SalePrice { get; init; }
        public decimal CurrentyQuantity { get; init; }
        public decimal Quantity { get; init; }
        public decimal ReservedQuantity { get; init; }
        public decimal? Weight { get; init; }
        public IList<SupplierResponse> Suppliers { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime Date { get; init; }
        public string Number { get; init; }
        public string EAN { get; init; }
    }

    public record SupplierResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }


}
