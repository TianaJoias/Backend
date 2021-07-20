namespace Domain.Portifolio
{
    public interface IProductRepository : IRepositoryWrite<Product>, IRepositoryRead<Product>
    {
    }
}
