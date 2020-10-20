using MediatR;

namespace WebApi.Aplication
{
    public interface IQuery<TResult> : IRequest<TResult>
    {

    }
}
