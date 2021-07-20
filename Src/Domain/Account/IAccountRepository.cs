namespace Domain.Account
{
    public interface IAccountRepository : IRepositoryWrite<Account>, IRepositoryRead<Account>
    {
    }
}
