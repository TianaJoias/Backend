using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain;
using Domain.Portifolio;
using Domain.Specification;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace Application
{
    public class ProductsQueryHandler : IQueryPagedHandler<ProductQuery, ProductQueryResult>,
        IQueryHandler<ProductQueryById, ProductQueryResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductStockRepository _productStock;

        public ProductsQueryHandler(IProductRepository productRepository, IProductStockRepository productStock)
        {
            _productRepository = productRepository;
            _productStock = productStock;
        }

        public async Task<Result<PagedList<ProductQueryResult>>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm) || it.SKU.Contains(request.SearchTerm) || it.Tags.Any(x => x.Name.Contains(request.SearchTerm));
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var spec = SpecifcationBuilder<Product>.Where(query).WithPage(request.PageNumber, request.PageSize).Sort(request.Sort).Build();
            var result = await _productRepository.Filter(spec);

            var ids = result.Select(it => it.Id);
            var stocks = await _productStock.Filter(it => ids.Contains(it.Id));
            var records = result.Join(stocks, it => it.Id, it => it.Id, (p, s) => new
            {
                p.Id,
                p.SKU,
                p.Description,
                Tags = p.Tags.Select(it => it.Id),
                s.Quantity,
                s.ReservedQuantity
            }).ToList();
            var rest = result.Where(it => !records.Any(r => r.Id == it.Id)).Select(p => new
            {
                p.Id,
                p.SKU,
                p.Description,
                Tags = p.Tags.Select(it => it.Id),
                Quantity = (decimal)0,
                ReservedQuantity = (decimal)0
            }).ToList();
            records.AddRange(rest);
            var records22 = records.Adapt<List<ProductQueryResult>>();

            var dto = new PagedList<ProductQueryResult>(records22, result.CurrentPage, result.TotalPages, result.TotalCount);
            return Result.Ok(dto);
        }


        public async Task<Result<ProductQueryResult>> Handle(ProductQueryById request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.Find(request.Id);
            return Result.Ok(result.Adapt<ProductQueryResult>());
        }
    }


    public record ProductQueryById(Guid Id) : IQuery<ProductQueryResult>;

    public record ProductQueryResult
    {
        public Guid? Id { get; init; }
        public string SKU { get; init; }
        public string Description { get; init; }
        public IList<Guid> Tags { get; init; }
        public decimal ReservedQuantity { get; set; }
        public decimal Quantity { get; set; }
    }
}
