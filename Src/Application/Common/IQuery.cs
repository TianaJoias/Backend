using FluentResults;
using MediatR;

namespace Application.Common
{
    public interface IQuery<TResult> : IRequest<Result<TResult>>
    {

    }
}
