using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Application.Specification;
using Domain;

namespace Application
{
    public interface IRepositoryRead<T> where T : IEntity
    {
        Task<bool> Contains(Expression<Func<T, bool>> filter);
        Task<bool> Contains(ISpecification<T> specification);
        Task<T> Find(Guid id);
        Task<T> Find(Expression<Func<T, bool>> filter);
        Task<T> Find(ISpecification<T> specification);
        Task<List<T>> Filter(Expression<Func<T, bool>> filter);
        Task<List<T>> Filter(ISpecification<T> specification);
        Task<PagedList<T>> FilterPaged(ISpecificationPaged<T> specification);
    }
}
