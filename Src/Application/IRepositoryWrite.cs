using System.Threading.Tasks;
using Domain;

namespace Application
{
    public interface IRepositoryWrite<T>: IRepositoryRead<T> where T : IEntity
    {
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }

    public enum SortDirection { Asc, Desc }
}
