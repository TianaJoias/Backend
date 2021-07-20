using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class ProductStockRepository : RepositoryBase<ProductStock>, IProductStockRepository
    {
        public ProductStockRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

    }
    public class SupplierProductRepository : RepositoryBase<SupplierProduct>, ISupplierProductRepository
    {
        public SupplierProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }

}
