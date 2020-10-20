using Domain;
using Domain.Portifolio;

namespace Infra.EF.Repositories
{
    public class TagRepository : RepositoryBase<Tag>, ITagRepository
    {
        public TagRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }
    }
}
