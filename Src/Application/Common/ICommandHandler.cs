using FluentResults;
using MediatR;

namespace Application.Common
{
    public interface ICommandHandler<TCommand> : IRequestHandler<TCommand, Result> where TCommand : ICommand
    {
    }
}
