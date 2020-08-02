using System;
using System.Threading.Tasks;
using WebApi.Domain;

namespace WebApi.Infra
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TianaJoiasContextDB _context;
        public UnitOfWork(TianaJoiasContextDB context)
        {
            _context = context;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Commit()
        {
            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task Add<T>(T entity) where T : class, IEntity
        {
            await _context.Set<T>().AddAsync(entity);
        }

        public Task Remove<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Remove(entity);
            return Task.CompletedTask;
        }

        public Task Update<T>(T entity) where T : class, IEntity
        {
            _context.Set<T>().Update(entity);
            return Task.CompletedTask;
        }
    }
}
