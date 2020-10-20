using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace WebApi.Aplication
{
    public interface IQueryHandler<TQuery, TResult> :
        IRequestHandler<TQuery, Result<TResult>>
        where TQuery : IQuery<Result<TResult>>
    {
    }

    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand> where TCommand : ICommand
    {
    }

    public interface ICommand : IRequest
    {
    }
}
