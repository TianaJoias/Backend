
namespace Domain.Stock
{
    public interface IProductStockRepository : IRepository<ProductStock>
    {
    }

    public interface ISupplierProductRepository : IRepository<SupplierProduct>, IRepositoryPagination<SupplierProduct>
    {
    }
}
