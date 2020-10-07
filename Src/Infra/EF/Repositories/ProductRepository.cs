using System.Linq;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Product> Load(IQueryable<Product> query)
        {
            return query.Include(it => it.Categories);
        }
    }
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }
    public class BatchRepository : RepositoryBase<Lot>, IBatchRepository
    {
        public BatchRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Lot> Load(IQueryable<Lot> query)
        {
            return query.Include(it => it.Suppliers);
        }
    }

    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }

    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }
}
