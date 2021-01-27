using System.Linq;
using Domain;
using Domain.Portifolio;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Product> Load(IQueryable<Product> query)
        {
            return query.Include(it => it.Categories);
        }
    }
}
