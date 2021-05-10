using System.Threading;
using System.Threading.Tasks;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierQueryHandler : IQueryHandler<ProductSupplierQuery, PagedData<ProductSupplierResult>>
    {
        private readonly ISupplierProductRepository _supplierProductRepository;

        public ProductSupplierQueryHandler(ISupplierProductRepository supplierProductRepository)
        {
            _supplierProductRepository = supplierProductRepository;
        }

        public async Task<Result<PagedData<ProductSupplierResult>>> Handle(ProductSupplierQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierProductRepository.GetPaged(it => it.Product.Id == request.ProductId, request.Page, request.PageSize, request.OrderBy);
            return Result.Ok(suppliers.Adapt<PagedData<ProductSupplierResult>>());
        }
    }
}
