using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain;
using Domain.Portifolio;
using Domain.Specification;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock.Queries
{
    public class LotSearchQueryHandler : IQueryHandler<LotSearchQuery, LotResult>, IQueryPagedHandler<LotsByProductIdQuery, LotResponse>
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
            var lot = await _lotRepository.Find(it => it.EAN.Equals(request.ean));
            if (lot is null)
                return Result.Ok<LotResult>(null);
            var product = await _productRepository.Find(lot.ProductId);
            var lotResult = product.Adapt<LotResult>();
            lotResult = lot.Adapt(lotResult);
            return Result.Ok(lotResult);
        }

        public async Task<Result<PagedList<LotResponse>>> Handle(LotsByProductIdQuery request, CancellationToken cancellationToken)
        {
            var spec = SpecifcationBuilder<Lot>.Where(it => it.ProductId == request.ProductId)
                .WithPage(request.PageNumber, request.PageSize)
                //.WithOrderBy() https://code-maze.com/sorting-aspnet-core-webapi/
                .Build();
            var lots = await _lotRepository.Filter(spec);
            var lotResult = lots.Adapt<PagedList<LotResponse>>();
            return Result.Ok(lotResult);
        }
    }

    public class LotsByProductIdQuery : QueryPaged<LotResponse>
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
