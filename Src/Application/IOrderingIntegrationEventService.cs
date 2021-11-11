using System.Threading.Tasks;
using BuildingBlocks.EventBus;

namespace Application
{
    public interface IOrderingIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync();
        Task AddAndSaveEventAsync(IntegrationEvent evt);
    }
}
