using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Account;
using Domain.Catalog;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Catalog.Queries
{
    public class AgentsQueryHandler : IQueryHandler<AgentsQuery, PagedData<AgentsQueryResult>>
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IAccountRepository _accountRepository;

        public AgentsQueryHandler(IAgentRepository agentRepository, IAccountRepository accountRepository)
        {
            _agentRepository = agentRepository;
            _accountRepository = accountRepository;
        }

        public async Task<Result<PagedData<AgentsQueryResult>>> Handle(AgentsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _agentRepository.List(it => it.AccountableId == request.AccountableId);
            var ids = paged.Select(it => it.Id);
            var x = await _accountRepository.List(it => ids.Contains(it.Id));
            var y = paged.Join(x, (it) => it.Id, it => it.Id, (agent, account) => new
            {
                agent.Id,
                account.Name,
                account.User.Email,
                agent.AccountableId,
                CurrentCatalogId = agent.CurrentCatalog?.Id
            });
            var records = y.Adapt<IList<AgentsQueryResult>>();
            var result = new PagedData<AgentsQueryResult>(1, 1, paged.Count, records);
            return Result.Ok(result);
        }
    }

    public class AgentsQuery : FilterPaged, IQuery<PagedData<AgentsQueryResult>>
    {
        public Guid AccountableId { get; set; }
    }

    public record AgentsQueryResult
    {
        public Guid AccountableId { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
    }
}
