using System.Linq;
using Domain;
using Domain.Stock;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class ProductStockRepository : RepositoryBase<ProductStock>, IProductStockRepository
    {
        public ProductStockRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<ProductStock> Load(IQueryable<ProductStock> query)
        {
            return query;
        }
    }
    public class SupplierProductRepository : RepositoryBase<SupplierProduct>, ISupplierProductRepository
    {
        public SupplierProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
        public override IQueryable<SupplierProduct> Load(IQueryable<SupplierProduct> query)
        {
            return query.Include(it => it.Product).Include(it => it.Supplier);
        }
    }
}
