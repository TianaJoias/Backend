using System.Linq;
using Domain;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class CatalogRepository : RepositoryBase<Catalog>, ICatalogRepository
    {
        public CatalogRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        {
        }

        public override IQueryable<Catalog> Load(IQueryable<Catalog> query)
        {
            return query.Include(it => it.Items).Include(it => it.Agent);
        }
    }
}
