using Microsoft.AspNetCore.Authentication;

namespace WebApi.Security.Custom
{
    public class CustomAuthOptions : AuthenticationSchemeOptions
    {
        public string UserInfoEndpoint { get; set; }
    }
}

