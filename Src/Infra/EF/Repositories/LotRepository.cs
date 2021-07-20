using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class LotRepository : RepositoryBase<Lot>, ILotRepository
    {
        public LotRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }
}
