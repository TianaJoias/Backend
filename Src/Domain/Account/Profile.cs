using System;

namespace Domain.Account
{
    public class Profile: BaseEntity
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Address Address { get; set; }
    }
}
