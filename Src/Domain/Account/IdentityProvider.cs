namespace Domain.Account
{
    public class IdentityProvider : BaseEntity
    {
        public string SubjectId { get; set; }
        public string Provider { get; set; }
    }
}
