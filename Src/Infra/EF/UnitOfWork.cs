using System;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infra.EF
{
    public sealed class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly TianaJoiasContextDB _context;
        private readonly IMediator _mediator;
        private readonly ILogger<UnitOfWork> _logger;

        public UnitOfWork(TianaJoiasContextDB context, IMediator mediator, ILogger<UnitOfWork> logger)
        {
            _context = context;
            _mediator = mediator;
            _logger = logger;
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
                foreach (var @event in events)
                    try
                    {
                        await _mediator.Publish(@event);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, $"Error on event {nameof(@event)}");
                    }
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
