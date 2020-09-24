namespace Domain
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

    public interface ITagRepository : IRepository<Tag>
    {
    }
}
