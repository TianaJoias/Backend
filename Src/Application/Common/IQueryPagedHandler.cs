using Domain;
using Domain.Specification;
using FluentResults;
using MediatR;

namespace Application.Common
{
    public interface IQueryPagedHandler<TQuery, TResult> :
    IRequestHandler<TQuery, Result<PagedList<TResult>>>
    where TQuery : IQuery<PagedList<TResult>>
    {
    }
}
