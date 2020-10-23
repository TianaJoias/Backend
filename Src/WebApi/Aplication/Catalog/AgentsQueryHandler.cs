using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Catalog;
using FluentResults;
using Mapster;

namespace WebApi.Aplication.Catalog
{
    public class AgentsQueryHandler : IQueryHandler<AgentsQuery, QueryPagedResult<AgentsQueryResult>>
    {
        private readonly IAgentRepository _agentRepository;

        public AgentsQueryHandler(IAgentRepository agentRepository)
        {
            _agentRepository = agentRepository;
        }

        public async Task<Result<QueryPagedResult<AgentsQueryResult>>> Handle(AgentsQuery request, CancellationToken cancellationToken)
        {
            var paged = await _agentRepository.GetPaged(it => true, request.Page, request.PageSize, request.OrderBy);
            var records = paged.Records.Adapt<IList<AgentsQueryResult>>();
            var result = new QueryPagedResult<AgentsQueryResult>(paged.CurrentPage, paged.PageCount, paged.PageSize, paged.RowCount, Records: records);
            return Result.Ok(result);
        }
    }

    public record AgentsQuery : PaginationQuery, IQuery<QueryPagedResult<AgentsQueryResult>>
    {

    }

    public record AgentsQueryResult
    {
        public Guid AccountableId { get; set; }
        public Guid Id { get; set; }
    }
}
