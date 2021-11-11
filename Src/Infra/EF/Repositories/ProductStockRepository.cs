using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class ProductStockRepository : RepositoryBase<StockUnit>, IProductStockRepository
    {
        public ProductStockRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }

    }
    public class SupplierProductRepository : RepositoryBase<ProductSupplier>, ISupplierProductRepository
    {
        public SupplierProductRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }
    }

}
