using Domain;
using Domain.Account;

namespace Infra.EF.Repositories
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IUnitOfWork unitOfWork, ProductContextDB context) : base(unitOfWork, context)
        { }
    }
}
