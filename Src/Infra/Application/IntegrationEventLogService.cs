using Microsoft.EntityFrameworkCore;
using System;
using BuildingBlocks.EventBus;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;

namespace Infra.Application
{
    public class IntegrationEventLogService : IIntegrationEventLogService, IDisposable
    {
        private readonly ApplicationContextDB _dbConnection;
        private readonly List<Type> _eventTypes;
        private volatile bool disposedValue;

        public IntegrationEventLogService(ApplicationContextDB dbConnection)
        {
            _dbConnection = dbConnection ?? throw new ArgumentNullException(nameof(dbConnection));

            _eventTypes = Assembly.Load(Assembly.GetEntryAssembly().FullName)
                .GetTypes()
                .Where(t => t.Name.EndsWith(nameof(IntegrationEvent)))
                .ToList();
        }

        public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(string transactionId)
        {
            var tid = transactionId.ToString();

            var result = await _dbConnection.Set<IntegrationEventLogEntry>()
                .Where(e => e.TransactionId == tid && e.State == EventStateEnum.NotPublished).ToListAsync();

            if (result != null && result.Any())
            {
                return result.OrderBy(o => o.CreationTime)
                    .Select(e => e.DeserializeJsonContent(_eventTypes.Find(t => t.Name == e.EventTypeShortName)));
            }

            return new List<IntegrationEventLogEntry>();
        }

        public Task SaveEventAsync(IntegrationEvent @event, string transactionId)
        {
            var eventLogEntry = new IntegrationEventLogEntry(@event, transactionId);

            //_integrationEventLogContext.Database.UseTransaction(transaction.GetDbTransaction());
            _dbConnection.Add(eventLogEntry);

            return _dbConnection.SaveChangesAsync();
        }

        public Task MarkEventAsPublishedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.Published);
        }

        public Task MarkEventAsInProgressAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.InProgress);
        }

        public Task MarkEventAsFailedAsync(Guid eventId)
        {
            return UpdateEventStatus(eventId, EventStateEnum.PublishedFailed);
        }

        private Task UpdateEventStatus(Guid eventId, EventStateEnum status)
        {
            var eventLogEntry = _dbConnection.Set<IntegrationEventLogEntry>().Single(ie => ie.EventId == eventId);
            eventLogEntry.State = status;

            if (status == EventStateEnum.InProgress)
                eventLogEntry.TimesSent++;

            _dbConnection.Update(eventLogEntry);

            return _dbConnection.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _dbConnection?.Dispose();
                }


                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
