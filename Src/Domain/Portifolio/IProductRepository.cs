namespace Domain.Portifolio
{
    public interface IProductRepository : IRepository<Product>, IRepositoryPagination<Product>
    {
    }
}
