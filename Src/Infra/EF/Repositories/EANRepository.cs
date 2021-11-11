using Domain;
using Domain.Stock;

namespace Infra.EF.Repositories
{
    public class EANRepository : RepositoryBase<Configuration>, IConfigurationRepository
    {
        public EANRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }

    }
}
