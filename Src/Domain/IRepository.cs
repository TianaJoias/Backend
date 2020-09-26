using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Domain
{
    public interface IRepository<T> where T : IEntity
    {
        Task<T> GetById(Guid id);
        Task<List<T>> List();
        Task<List<T>> List(Expression<Func<T, bool>> filter);
        Task<bool> Exists(Expression<Func<T, bool>> filter);
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
    public interface IRepositoryPagination<T> where T : IEntity
    {
       Task<PagedResult<T>> GetPaged(Expression<Func<T, bool>> filter,
                        int page, int pageSize, Dictionary<string, Sort> ordering = null);
    }

    public enum Sort { Asc, Desc }
    public record PagedResult<T>(int CurrentPage, int PageCount, int PageSize, int RowCount, IList<T> Records);
}
