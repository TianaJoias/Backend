using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application
{
    public class IdentifiedCommandHandler<T> : IRequestHandler<IdentifiedCommand<T>, Result>
       where T : ICommand
    {
        private readonly IMediator _mediator;
        private readonly IRequestManager _requestManager;
        private readonly ILogger<IdentifiedCommandHandler<T>> _logger;
        private readonly IOrderingIntegrationEventService _orderingIntegrationEventService;

        public IdentifiedCommandHandler(
            IMediator mediator,
            IRequestManager requestManager,
            ILogger<IdentifiedCommandHandler<T>> logger)
        {
            _mediator = mediator;
            _requestManager = requestManager;
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates the result value to return if a previous request was found
        /// </summary>
        /// <returns></returns>
        protected virtual Result CreateResultForDuplicateRequest()
        {
            return default;
        }

        /// <summary>
        /// This method handles the command. It just ensures that no other request exists with the same ID, and if this is the case
        /// just enqueues the original inner command.
        /// </summary>
        /// <param name="message">IdentifiedCommand which contains both original command & request ID</param>
        /// <returns>Return value of inner command or default value if request same ID was found</returns>
        public async Task<Result> Handle(IdentifiedCommand<T> message, CancellationToken cancellationToken)
        {
            //TODO: tratar lock distribuido para concorrencia.
            var alreadyExists = await _requestManager.ExistAsync(message.Id);
            if (alreadyExists)
                return CreateResultForDuplicateRequest();

            await _requestManager.CreateRequestForCommandAsync<T>(message.Id);
            try
            {
                var command = message.Command;

                // Send the embeded business command to mediator so it runs its related CommandHandler 
                var result = await _mediator.Send(command, cancellationToken);
                await _orderingIntegrationEventService.PublishEventsThroughEventBusAsync();
                return result;
            }
            catch (Exception ex)
            {
                return Result.Fail(ex.Message);
            }
        }
    }
}
