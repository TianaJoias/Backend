using Domain;
using Domain.Portifolio;

namespace Infra.EF.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }

    }
}
