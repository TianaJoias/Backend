
using System;

namespace Application
{
    public interface IIdentityService
    {
        Guid GetUserIdentity();
        string GetUserEmail();
    }

    public struct SCOPES {
        public const string USER = "access_as_user";
        public const string ADMIN = "access_as_admin";
    }
}
