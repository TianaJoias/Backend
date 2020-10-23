using System;

namespace Domain.Catalog
{
    public class Agent : BaseEntity
    {
        public Guid AccountableId { get; set; }
        public Catalog CurrentCatalog { get; set; }
        public Agent(Guid ownerId, Guid accountableId)
        {
            Id = ownerId;
            AccountableId = accountableId;
        }

        protected Agent()
        {

        }

        public void SetCurrentCatalog(Catalog catalog)
        {
            CurrentCatalog = catalog;
        }
    }
}
