namespace Domain.Catalog
{
    public interface ICatalogRepository : IRepository<Catalog>, IRepositoryPagination<Catalog>
    {
    }
    public interface IAgentRepository : IRepository<Agent>, IRepositoryPagination<Agent>
    {
    }
}
