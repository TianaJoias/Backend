using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Portifolio;
using FluentResults;
using Mapster;

namespace WebApi.Aplication
{
    public class ProductsQueryHandler : IQueryHandler<ProductQuery, PagedResult<ProductQueryResult>>,
        IQueryHandler<ProductQueryById, ProductQueryResult>
    {
        private readonly IProductRepository _productRepository;

        public ProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<PagedResult<ProductQueryResult>>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm);
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var result = await _productRepository.GetPaged(query, request.Page, request.PageSize,
                request.OrderBy);

            var dto = new PagedResult<ProductQueryResult>(result.CurrentPage, result.PageCount, result.PageSize, result.RowCount, Records: result.Records.Adapt<IList<ProductQueryResult>>());
            return Result.Ok(dto);
        }


        public async Task<Result<ProductQueryResult>> Handle(ProductQueryById request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetById(request.Id);
            return Result.Ok(result.Adapt<ProductQueryResult>());
        }
    }


    public record ProductQueryById(Guid Id) : IQuery<Result<ProductQueryResult>>;

    public record ProductQueryResult
    {
        public Guid? Id { get; init; }
        public string BarCode { get; init; }
        public string Description { get; init; }
        public IList<Guid> Categories { get; init; }
    }
}
