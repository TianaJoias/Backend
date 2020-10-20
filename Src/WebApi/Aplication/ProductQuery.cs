using Domain;
using FluentResults;

namespace WebApi.Aplication
{
    public class ProductQuery : PaginationQuery, IQuery<Result<PagedResult<ProductQueryResult>>>
    {

    }
}
