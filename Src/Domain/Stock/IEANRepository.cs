namespace Domain.Stock
{
    public interface IEANRepository : IRepositoryWrite<EAN>, IRepositoryRead<EAN>
    {
    }
}
