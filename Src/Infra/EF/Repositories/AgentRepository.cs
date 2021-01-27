using System.Linq;
using Domain;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class AgentRepository : RepositoryBase<Agent>, IAgentRepository
    {
        public AgentRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        {
        }

        public override IQueryable<Agent> Load(IQueryable<Agent> query)
        {
            return query.Include(it => it.CurrentCatalog);
        }
    }
}
