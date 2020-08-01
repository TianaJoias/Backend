using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace WebApi.Infra
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
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task<List<T>> List()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<List<T>> List(Expression<Func<T, bool>> filter)
        {

            return await _context.Set<T>().Where(filter).ToListAsync();
        }
        public async Task<bool> Exists(Expression<Func<T, bool>> filter)
        {

            return await _context.Set<T>().AnyAsync(filter);
        }
        public Task Update(T entity)
        {
            return _unitOfWork.Update(entity);
        }
    }
}
