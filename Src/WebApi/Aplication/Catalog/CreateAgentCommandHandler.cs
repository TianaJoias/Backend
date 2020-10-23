using System;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Catalog;
using FluentResults;

namespace WebApi.Aplication.Catalog
{
    public class CreateAgentCommandHandler : ICommandHandler<CreateAgentCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAgentCommandHandler(IAgentRepository agentRepository, IUnitOfWork unitOfWork)
        {
            _agentRepository = agentRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            var agent = new Agent(request.OwnerId, request.AccountableId);
            await _agentRepository.Add(agent);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record CreateAgentCommand(Guid OwnerId, Guid AccountableId) : ICommand;

}
