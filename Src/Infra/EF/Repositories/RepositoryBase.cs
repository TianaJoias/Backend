using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using Domain.Specification;

namespace Infra.EF.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryRead<T>, IRepositoryWrite<T> where T : class, IEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TianaJoiasContextDB _context;

        public RepositoryBase(IUnitOfWork unitOfWork, TianaJoiasContextDB context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        protected DbSet<T> Set()
        {
            return _context.Set<T>();
        }
        public Task<T> Add(T entity)
        {
            _unitOfWork.Add(entity);
            return Task.FromResult(entity);
        }

        public Task Delete(T entity)
        {
            return _unitOfWork.Remove(entity);
        }

        public async Task<T> Find(Guid id)
        {
            return await Find(it => it.Id == id);
        }

        public async Task<List<T>> All()
        {
            return await Set().ToListAsync();
        }

        public async Task<List<T>> Filter(Expression<Func<T, bool>> filter)
        {

            return await Set().Where(filter).ToListAsync();
        }

        public async Task<T> Find(Expression<Func<T, bool>> filter)
        {
            return await Set().FirstOrDefaultAsync(filter);
        }

        public Task Update(T entity)
        {
            return _unitOfWork.Update(entity);
        }

        public Task<List<T>> Filter(ISpecification<T> specification)
        {
            return SpecificationEvaluator<T>.GetQuery(Set().AsQueryable(), specification).ToListAsync();
        }

        public async Task<PagedList<T>> Filter(ISpecificationPaged<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(Set().AsQueryable(), specification);

            var resultPage = query.PageResult(specification.PageNumber, specification.PageSize);
            var records = await resultPage.Queryable.ToListAsync();
            return new PagedList<T>(records, resultPage.RowCount, resultPage.CurrentPage, resultPage.PageSize);
        }

        public Task<bool> Contains(Expression<Func<T, bool>> filter)
        {
            return Set().AnyAsync(filter);
        }

        public Task<bool> Contains(ISpecification<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(Set().AsQueryable(), specification);
            return query.AnyAsync();
        }

        public Task<T> Find(ISpecification<T> specification)
        {
            var query = SpecificationEvaluator<T>.GetQuery(Set().AsQueryable(), specification);
            return query.FirstOrDefaultAsync();
        }
    }
}
