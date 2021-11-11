using System;
using System.Threading;
using System.Threading.Tasks;

namespace Application
{
    public interface IUnitOfWork
    {
        Task<bool> Commit(CancellationToken cancellationToken = default);
    }
}
