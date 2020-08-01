using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Infra
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
}
