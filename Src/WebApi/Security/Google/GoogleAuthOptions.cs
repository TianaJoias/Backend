using Microsoft.AspNetCore.Authentication;

namespace WebApi.Security.Google
{
    public class GoogleAuthOptions : AuthenticationSchemeOptions
    {
        public string UserInfoEndpoint { get; set; }
    }
}

