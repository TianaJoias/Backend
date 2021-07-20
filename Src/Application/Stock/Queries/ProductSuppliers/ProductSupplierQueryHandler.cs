using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using Domain;
using Domain.Specification;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace Application.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierQueryHandler : IQueryPagedHandler<ProductSupplierQuery, ProductSupplierResult>
    {
        private readonly ISupplierProductRepository _supplierProductRepository;

        public ProductSupplierQueryHandler(ISupplierProductRepository supplierProductRepository)
        {
            _supplierProductRepository = supplierProductRepository;
        }

        public async Task<Result<PagedList<ProductSupplierResult>>> Handle(ProductSupplierQuery request, CancellationToken cancellationToken)
        {
            var spec = SpecifcationBuilder<SupplierProduct>.Where(it => it.Product.Id == request.ProductId).WithPage(request.PageNumber, request.PageSize).Build();
            var suppliers = await _supplierProductRepository.Filter(spec);
            return Result.Ok(suppliers.Adapt<PagedList<ProductSupplierResult>>());
        }
    }
}
