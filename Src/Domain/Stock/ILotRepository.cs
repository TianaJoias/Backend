namespace Domain.Stock
{
    public interface ILotRepository : IRepositoryWrite<Lot>, IRepositoryRead<Lot>
    {
    }
}
