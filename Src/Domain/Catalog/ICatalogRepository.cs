namespace Domain.Catalog
{
    public interface ICatalogRepository : IRepositoryWrite<Catalog>, IRepositoryRead<Catalog>
    {
    }
    public interface IAgentRepository : IRepositoryWrite<Agent>, IRepositoryRead<Agent>
    {
    }
}
