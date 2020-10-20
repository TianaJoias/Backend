using Domain;
using Domain.Catalog;

namespace Infra.EF.Repositories
{
    public class ChannelRepository : RepositoryBase<Channel>, IChannelRepository
    {
        public ChannelRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        {
        }
    }
}
