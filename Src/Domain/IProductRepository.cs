namespace Domain
{
    public interface IProductRepository : IRepository<Product>, IRepositoryPagination<Product>
    {
    }

    public interface IBatchRepository : IRepository<Lot>
    {
    }

    public interface ISupplierRepository : IRepository<Supplier>
    {
    }

    public interface ITagRepository : IRepository<Tag>
    {
    }

    public interface IAccountRepository : IRepository<Account>
    {
    }
}
