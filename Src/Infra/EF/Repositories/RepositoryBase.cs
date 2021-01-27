using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace Infra.EF.Repositories
{
    public abstract class RepositoryBase<T> : IRepositoryPagination<T>, IRepository<T> where T : class, IEntity
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly TianaJoiasContextDB _context;

        public RepositoryBase(IUnitOfWork unitOfWork, TianaJoiasContextDB context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public virtual IQueryable<T> Load(IQueryable<T> query)
        {
            return query;
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

        public async Task<T> GetById(Guid id)
        {
            return await GetByQuery(it => it.Id == id);
        }

        public async Task<List<T>> List()
        {
            return await Load(_context.Set<T>()).ToListAsync();
        }

        public async Task<List<T>> List(Expression<Func<T, bool>> filter)
        {

            return await Load(_context.Set<T>()).Where(filter).ToListAsync();
        }
        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {

            return await _context.Set<T>().AnyAsync(filter);
        }

        public async Task<T> GetByQuery(Expression<Func<T, bool>> filter)
        {
            return await Load(_context.Set<T>()).FirstOrDefaultAsync(filter);
        }

        public Task Update(T entity)
        {
            return _unitOfWork.Update(entity);
        }

        public async Task<Domain.PagedResult<T>> GetPaged(Expression<Func<T, bool>> filter,
                                int page, int pageSize, Dictionary<string, Sort> ordering = null)
        {
            var query = _context.Set<T>().Where(filter);
            var then = false;
            if (ordering is not null)
                foreach (var item in ordering)
                {
                    var sort = item.Value == Sort.Desc ? string.Empty : "descending";
                    if (then)
                        query = (query as IOrderedQueryable<T>).ThenBy($"{item.Key} {sort}");
                    else
                        query = query.OrderBy($"{item.Key} {sort}");
                    then = true;
                }
            var resultPage = query.PageResult(page, pageSize);
            var records = await Load(resultPage.Queryable).ToListAsync();
            return new Domain.PagedResult<T>(resultPage.CurrentPage, resultPage.PageCount, resultPage.PageSize, resultPage.RowCount , records);
        }


    }
}
