using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BuildingBlocks.EventBus
{
    public interface IIntegrationEventLogService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string transactionId);
        Task SaveEventAsync(IntegrationEvent @event, string transactionId);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
    }

}
