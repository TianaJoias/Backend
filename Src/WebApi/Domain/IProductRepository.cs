namespace WebApi.Domain
{
    public interface IProductRepository : IRepository<Product>
    {
    }

    public interface IBatchRepository : IRepository<Batch>
    {
    }

    public interface ISupplierRepository : IRepository<Supplier>
    {
    }
}
