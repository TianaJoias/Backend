using System.Linq;
using Domain;
using Domain.Stock;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class LotRepository : RepositoryBase<Lot>, ILotRepository
    {
        public LotRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Lot> Load(IQueryable<Lot> query)
        {
            return query.Include(it => it.Suppliers);
        }
    }
}
