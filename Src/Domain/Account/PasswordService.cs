using System.Threading.Tasks;
using Crypt = BCrypt.Net.BCrypt;

namespace Domain.Account
{
    public class PasswordService: IPasswordService
    {
        public Task<bool> Verify(string text, string hash)
        {
            return Task.FromResult(Crypt.EnhancedVerify(text ?? "", hash ?? "", BCrypt.Net.HashType.SHA384));
        }

        public Task<string> Hash(string text)
        {
            return Task.FromResult(Crypt.EnhancedHashPassword(text, BCrypt.Net.HashType.SHA384, 11));
        }
    }
}
