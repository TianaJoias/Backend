using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Mapster;
using MediatR;

namespace WebApi.Aplication
{
    public class ProductsQuery : IQuery<ProductQueryInDTO, PagedResult<ProductDTO>>,
        IQuery<ProductQueryByIdRequest, ProductDTO>
    {
        private readonly IProductRepository _productRepository;

        public ProductsQuery(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<PagedResult<ProductDTO>> Handle(ProductQueryInDTO request, CancellationToken cancellationToken)
        {
            Expression<Func<Product, bool>> query = it => it.Description.Contains(request.SearchTerm);
            if (string.IsNullOrWhiteSpace(request.SearchTerm))
                query = it => true;
            var result = await _productRepository.GetPaged(query, request.Page, request.PageSize,
                request.OrderBy);

            var dto = new PagedResult<ProductDTO>(result.CurrentPage, result.PageCount, result.PageSize, result.RowCount, Records: result.Records.Adapt<IList<ProductDTO>>());
            return dto;
        }


        public async Task<ProductDTO> Handle(ProductQueryByIdRequest request, CancellationToken cancellationToken)
        {
            var result = await _productRepository.GetById(request.Id);
            return result.Adapt<ProductDTO>();
        }
    }

    public class PaginationQuery
    {
        public string SearchTerm { get; set; }
        public int Page { get; set; } = 1; public int PageSize { get; set; } = 5; public Dictionary<string, Sort> OrderBy { get; set; } = null;
    }


    public record ProductQueryByIdRequest(Guid Id) : IRequest<ProductDTO>;

    public record ProductDTO
    {
        public Guid? Id { get; init; }
        public string BarCode { get; init; }
        public string Description { get; init; }
        public IList<Guid> Categories { get; init; }
    }

    public interface IQuery<TInput, TOutput> : IRequestHandler<TInput, TOutput> where TInput : IRequest<TOutput>
    {
    }
}
