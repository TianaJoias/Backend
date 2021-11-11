using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class SupplierRepository : RepositoryBase<Supplier>, ISupplierRepository
    {
        public SupplierRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }
    }
}
