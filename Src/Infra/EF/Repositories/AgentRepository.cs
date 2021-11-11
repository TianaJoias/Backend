using Domain;
using Domain.Catalog;

namespace Infra.EF.Repositories
{
    public class AgentRepository : RepositoryBase<Agent>, IAgentRepository
    {
        public AgentRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        {
        }
    }
}
