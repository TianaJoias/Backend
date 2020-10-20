namespace Domain.Catalog
{
    public interface ICatalogRepository : IRepository<Catalog>
    {
    }
    public interface IAgentRepository : IRepository<Agent>, IRepositoryPagination<Agent>
    {
    }
}
