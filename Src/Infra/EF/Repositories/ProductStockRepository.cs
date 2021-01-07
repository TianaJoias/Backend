using System.Linq;
using Domain;
using Domain.Stock;
using Microsoft.EntityFrameworkCore;

namespace Infra.EF.Repositories
{
    public class ProductStockRepository : RepositoryBase<ProductStock>, IProductStockRepository
    {
        public ProductStockRepository(IUnitOfWork unitOfWork, TianaJoiasContextDB context) : base(unitOfWork, context)
        { }

        public override IQueryable<Domain.Stock.ProductStock> Load(IQueryable<Domain.Stock.ProductStock> query)
        {
            return query;
        }
    }
}
