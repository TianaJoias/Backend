using FluentResults;
using MediatR;

namespace Application.Common
{
    public interface ICommand : IRequest<Result>
    {
    }
}
