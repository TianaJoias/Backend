using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Domain.Specification;

namespace Domain
{
    public interface IRepositoryWrite<T> where T : IEntity
    {
        Task<T> Add(T entity);
        Task Update(T entity);
        Task Delete(T entity);
    }
    public interface IRepositoryRead<T> where T : IEntity
    {
        Task<bool> Contains(Expression<Func<T, bool>> filter);
        Task<bool> Contains(ISpecification<T> specification);
        Task<T> Find(Guid id);
        Task<T> Find(Expression<Func<T, bool>> filter);
        Task<T> Find(ISpecification<T> specification);
        Task<List<T>> Filter(Expression<Func<T, bool>> filter);
        Task<List<T>> Filter(ISpecification<T> specification);
        Task<PagedList<T>> Filter(ISpecificationPaged<T> specification);
    }

    public enum SortDirection { Asc, Desc }
}
