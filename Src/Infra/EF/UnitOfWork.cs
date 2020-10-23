using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;

namespace Infra.EF
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TianaJoiasContextDB _context;
        private readonly IMediator _mediator;

        public UnitOfWork(TianaJoiasContextDB context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        public async Task<bool> Commit()
        {
            try
            {
                var entidades = _context.ChangeTracker.Entries<BaseEntity>().Select(it => it.Entity);

                await _context.SaveChangesAsync();
                var events = entidades.SelectMany(it => it.Events).ToList();
                foreach (var entity in entidades)
                    entity.ClearEvents();
                var tasks = events.Select(it => _mediator.Publish(it)).ToArray();
                Task.WaitAll(tasks);
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
