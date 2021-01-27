using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class EANRepository : RepositoryBase<EAN>, IEANRepository
    {
        public EANRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

    }
}
