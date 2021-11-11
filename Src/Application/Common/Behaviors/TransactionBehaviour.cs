using MediatR;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.Logging;
using System;
using BuildingBlocks.EventBus;

namespace Application.Common.Behaviors
{
    public class TransactionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
         where TRequest : ICommand
    
    {
        private readonly ILogger<TransactionBehaviour<TRequest, TResponse>> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public TransactionBehaviour(IUnitOfWork unitOfWork,
            IOrderingIntegrationEventService orderingIntegrationEventService,
            ILogger<TransactionBehaviour<TRequest, TResponse>> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _orderingIntegrationEventService = orderingIntegrationEventService ?? throw new ArgumentNullException(nameof(orderingIntegrationEventService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var response = default(TResponse);
            var typeName = request.GetGenericTypeName();
            try
            {
                if (_unitOfWork.HasTransaction)
                {
                    return await next();
                }

                await _unitOfWork.Transaction(async () =>
                  {
                      _logger.LogInformation("----- Begin transaction {TransactionId} for {CommandName} ({@Command})", transactionId, typeName, request);

                      response = await next();

                      _logger.LogInformation("----- Commit transaction {TransactionId} for {CommandName}", transactionId, typeName);

                  });
                await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync();
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ERROR Handling transaction for {CommandName} ({@Command})", typeName, request);

                throw;
            }
        }
    }
}
