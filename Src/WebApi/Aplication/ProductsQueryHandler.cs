using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain.Portifolio;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication
{
    public class ProductsQueryHandler : IQueryHandler<ProductQuery, PagedData<ProductQueryResult>>,
        IQueryHandler<ProductQueryById, ProductQueryResult>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductStockRepository _productStock;

        public ProductsQueryHandler(IProductRepository productRepository, IProductStockRepository productStock)
        {
            _productRepository = productRepository;
            _productStock = productStock;
        }

        public async Task<Result<PagedData<ProductQueryResult>>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm) || it.SKU.Contains(request.SearchTerm) || it.Tags.Any(x => x.Name.Contains(request.SearchTerm));
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var result = await _productRepository.GetPaged(query, request.Page, request.PageSize,
                request.OrderBy);

            var ids = result.Data.Select(it => it.Id);
            var stocks = await _productStock.List(it => ids.Contains(it.Id));
            var records = result.Data.Join(stocks, it => it.Id, it => it.Id, (p, s) => new
            {
                p.Id,
                p.SKU,
                p.Description,
                Tags = p.Tags.Select(it => it.Id),
                s.Quantity,
                s.ReservedQuantity
            }).ToList();
            var rest = result.Data.Where(it => !records.Any(r => r.Id == it.Id)).Select(p => new
            {
                p.Id,
                p.SKU,
                p.Description,
                Tags = p.Tags.Select(it => it.Id),
                Quantity = (decimal)0,
                ReservedQuantity = (decimal)0
            }).ToList();
            records.AddRange(rest);
            var records22 = records.Adapt<IList<ProductQueryResult>>();

            var dto = new PagedData<ProductQueryResult>(result.CurrentPage, result.TotalPages, result.TotalRows, records22);
            return Result.Ok(dto);
        }


        public async Task<Result<ProductQueryResult>> Handle(ProductQueryById request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetById(request.Id);
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
