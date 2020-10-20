using FluentResults;
using MediatR;

namespace WebApi.Aplication
{
    public interface IQuery<TResult> : IRequest<Result<TResult>>
    {

    }
}
