using FluentResults;
using MediatR;

namespace Application.Common
{
    public interface IQueryHandler<TQuery, TResult> :
        IRequestHandler<TQuery, Result<TResult>>
        where TQuery : IQuery<TResult>
    {
    }
}
