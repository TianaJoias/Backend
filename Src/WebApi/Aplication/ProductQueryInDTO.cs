using Domain;
using MediatR;

namespace WebApi.Aplication
{
    public class ProductQueryInDTO : PaginationQuery, IRequest<PagedResult<ProductDTO>>
    {

    }
}
