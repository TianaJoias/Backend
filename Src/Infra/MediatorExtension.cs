using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Infra
{
    static class MediatorExtension
    {
        public static async Task DispatchDomainEventsAsync(this IMediator mediator, DbContext ctx, System.Threading.CancellationToken cancellationToken)
        {
            var domainEntities = ctx.ChangeTracker
                .Entries<BaseEntity>()
                .Where(x => x.Entity.Events != null && x.Entity.Events.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.Events).Where(it => !it.Published).ToList();

            foreach (var domainEvent in domainEvents)
                await mediator.Publish(domainEvent, cancellationToken);
        }
    }
}
