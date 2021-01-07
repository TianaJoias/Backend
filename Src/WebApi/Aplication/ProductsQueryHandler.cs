using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Portifolio;
using FluentResults;
using Mapster;

namespace WebApi.Aplication
{
    public class ProductsQueryHandler : IQueryHandler<ProductQuery, QueryPagedResult<ProductQueryResult>>,
        IQueryHandler<ProductQueryById, ProductQueryResult>
    {
        private readonly IProductRepository _productRepository;

        public ProductsQueryHandler(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<Result<QueryPagedResult<ProductQueryResult>>> Handle(ProductQuery request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm) || it.SKU.Contains(request.SearchTerm) || it.Categories.Any(x => x.Tag.Name.Contains(request.SearchTerm));
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var result = await _productRepository.GetPaged(query, request.Page, request.PageSize,
                request.OrderBy);

            var dto = new QueryPagedResult<ProductQueryResult>(result.CurrentPage, result.PageCount, result.PageSize, result.RowCount, Records: result.Records.Adapt<IList<ProductQueryResult>>());
            return Result.Ok(dto);
        }


        public async Task<Result<ProductQueryResult>> Handle(ProductQueryById request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetById(request.Id);
            return Result.Ok(result.Adapt<ProductQueryResult>());
        }
    }


    public record QueryPagedResult<T>(int CurrentPage, int PageCount, int PageSize, int RowCount, IList<T> Records) : IQuery<T>;

    public record ProductQueryById(Guid Id) : IQuery<ProductQueryResult>;

    public record ProductQueryResult
    {
        public Guid? Id { get; init; }
        public string SKU { get; init; }
        public string Description { get; init; }
        public IList<Guid> Categories { get; init; }
    }
}
