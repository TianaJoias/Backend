using System;
using System.Linq;
using Domain.Account;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Security
{
    public class Policies
    {
        public static AuthorizationPolicy AdminPolicy() { return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Roles.ADMIN.ToString("G")).Build(); }
        public static AuthorizationPolicy UserPolicy() { return new AuthorizationPolicyBuilder().RequireAuthenticatedUser().RequireRole(Roles.USER.ToString("G")).Build(); }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class AuthorizeEnumAttribute : AuthorizeAttribute
    {
        public AuthorizeEnumAttribute(params object[] roles)
        {
            if (roles.Any(r => r.GetType() != typeof(Roles)))
                throw new ArgumentException(null, nameof(roles));

            Roles = string.Join(",", roles.Cast<Roles>().Select(r => r.ToString("G")));
        }
    }
}

