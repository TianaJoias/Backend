using System.Threading.Tasks;

namespace Domain
{
    public interface IUnitOfWork
    {
        Task<bool> Commit();
        Task Add<T>(T entity) where T : class, IEntity;
        Task Remove<T>(T entity) where T : class, IEntity;
        Task Update<T>(T entity) where T : class, IEntity;
    }
}
