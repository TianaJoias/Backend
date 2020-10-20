using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;

namespace WebApi.Aplication
{
    public interface IQueryHandler<TQuery, TResult> :
        IRequestHandler<TQuery, Result<TResult>>
        where TQuery : IQuery<TResult>
    {
    }

    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand
    {
    }

    public interface ICommand : IRequest<Result>
    {
    }
}
