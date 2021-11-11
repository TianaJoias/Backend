using System.Collections.Generic;

namespace Domain.Account
{
    public class Account : BaseEntity
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public IReadOnlyList<Profile> Profiles { get; private set; }
    }
}
