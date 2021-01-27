using Microsoft.AspNetCore.Authentication;

namespace WebApi.Security.Google
{
    public static class AddGoogleAuthenticationExtensions
    {
        public static AuthenticationBuilder AddGoogleAuthentication(this AuthenticationBuilder services)
        {
            services.AddScheme<GoogleAuthOptions, GoogleAuthenticationHandler>(GoogleAuthenticationDefaults.AuthenticationScheme, o => o.UserInfoEndpoint = "");

            return services;
        }

    }
}

