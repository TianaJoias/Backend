using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Security.Custom
{
    public class TokenConstants
    {
        public static string Issuer = "thisismeyouknow";
        public static string Audience = "thisismeyouknow";
        public static int ExpiryInMinutes = 1440;
        public static string key = "thiskeyisverylargetobreak";

        public static string EncryptingKey = "SecretKeySecretKeySecretKeySecretKeySecretKeySecretKeySecretKeyS";

        public static SecurityKey IssuerSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        public static SecurityKey EncryptionSecurityKey { get; } = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(EncryptingKey));
    }
}

