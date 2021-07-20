
namespace Domain.Stock
{
    public interface IProductStockRepository : IRepositoryWrite<ProductStock>, IRepositoryRead<ProductStock>
    {
    }

    public interface ISupplierProductRepository : IRepositoryWrite<SupplierProduct>, IRepositoryRead<SupplierProduct>
    {
    }
}
