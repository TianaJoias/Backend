using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Stock;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Stock.Queries.ProductSuppliers
{
    public class ProductSupplierQueryHandler : IQueryHandler<ProductSupplierQuery, IList<ProductSupplierResult>>
    {
        private readonly ISupplierProductRepository _supplierProductRepository;

        public ProductSupplierQueryHandler(ISupplierProductRepository supplierProductRepository)
        {
            _supplierProductRepository = supplierProductRepository;
        }

        public async Task<Result<IList<ProductSupplierResult>>> Handle(ProductSupplierQuery request, CancellationToken cancellationToken)
        {
            var suppliers = await _supplierProductRepository.List(it => it.Product.Id == request.ProductId);
            return Result.Ok(suppliers.Adapt<IList<ProductSupplierResult>>());
        }
    }
}
