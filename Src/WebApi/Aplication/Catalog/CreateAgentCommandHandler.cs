using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain;
using Domain.Account;
using Domain.Catalog;
using FluentResults;

namespace WebApi.Aplication.Catalog
{
    public class CreateAgentCommandHandler : ICommandHandler<CreateAgentCommand>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAgentCommandHandler(IAgentRepository agentRepository, IAccountRepository accountRepository,
        IPasswordService passwordService,
        IUnitOfWork unitOfWork)
        {
            _agentRepository = agentRepository;
            _accountRepository = accountRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
        }
        public async Task<Result> Handle(CreateAgentCommand request, CancellationToken cancellationToken)
        {
            var exists = await _accountRepository.Exists(it => it.User.Email.ToUpper() == request.Email.ToUpper());
            if (exists)
                return Result.Fail("Email already exists.");
            var password = await _passwordService.Hash("password");
            var account = new Domain.Account.Account
            {
                Name = request.Name,
                Roles = new List<Roles> { Roles.AGENT },
                User = new User
                {
                    Email = request.Email,
                    Password = password
                }
            };
            await _accountRepository.Add(account);
            var agent = new Agent(account.Id, request.AccountableId);
            await _agentRepository.Add(agent);
            await _unitOfWork.Commit();
            return Result.Ok();
        }
    }

    public record CreateAgentCommand(string Name, string Email, Guid AccountableId) : ICommand;

}
