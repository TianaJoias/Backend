using System.Threading.Tasks;

namespace BuildingBlocks.EventBus
{
    public interface IDynamicIntegrationEventHandler
    {
        Task Handle(dynamic eventData);
    }

}
