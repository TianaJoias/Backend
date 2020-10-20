using System;
using System.Collections.Generic;

namespace Domain.Catalog
{
    public class Agent : BaseEntity
    {
        public Guid AccountableId { get; set; }
        public Guid OwnerId { get; set; }
        public Catalog CurrentCatalog { get; set; }

        public void SetCurrentCatalog(Catalog catalog)
        {
            CurrentCatalog = catalog;
        }
    }
}
