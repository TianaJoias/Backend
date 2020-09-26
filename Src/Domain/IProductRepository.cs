namespace Domain
{
    public interface IProductRepository : IRepository<Product>, IRepositoryPagination<Product>
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
