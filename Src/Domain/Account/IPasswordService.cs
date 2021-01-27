using System.Threading.Tasks;

namespace Domain.Account
{
    public interface IPasswordService
    {
        Task<bool> Verify(string text, string hash);
        Task<string> Hash(string text);
    }
}
