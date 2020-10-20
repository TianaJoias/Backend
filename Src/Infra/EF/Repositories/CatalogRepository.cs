using Domain;
using Domain.Catalog;

namespace Infra.EF.Repositories
{
    public class CatalogRepository : RepositoryBase<Catalog>, ICatalogRepository
    {
        public CatalogRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        {
        }
    }
}
