using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using WebApi.Domain;

namespace WebApi.Infra.Repositories
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class, IEntity
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
            return await Load(_context.Set<T>()).FirstOrDefaultAsync(it=> it.Id == id);
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
        public Task Update(T entity)
        {
            return _unitOfWork.Update(entity);
        }
        public async Task<PagedResult<T>> GetPaged(Expression<Func<T, bool>> filter,
                                        int page, int pageSize, Func<IQueryable<T>, IOrderedQueryable<T>> ordering = null)
        {
            var query = _context.Set<T>().Where(filter);
            var result = new PagedResult<T>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = await query.CountAsync()
            };

            query = ordering == null ? query : ordering(query);
            var pageCount = (double)result.RowCount / pageSize;
            result.PageCount = (int)Math.Ceiling(pageCount);


            var skip = page * pageSize;
            result.Results = await Load(query.Skip(skip).Take(pageSize)).ToListAsync();
            return result;
        }
    }
}
