using Domain;
using FluentResults;

namespace WebApi.Aplication
{
    public record ProductQuery : PaginationQuery, IQuery<QueryPagedResult<ProductQueryResult>>
    {

    }
}
