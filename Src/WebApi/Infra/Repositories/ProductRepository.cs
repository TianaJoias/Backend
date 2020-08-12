using System.Linq;
using Microsoft.EntityFrameworkCore;
using WebApi.Domain;

namespace WebApi.Infra.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }

    public class BatchRepository : RepositoryBase<Batch>, IBatchRepository
    {
        public BatchRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Batch> Load(IQueryable<Batch> query)
        {
            return query.Include(it=> it.Product).Include(it=> it.Suppliers);
        }
    }

    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }
}
