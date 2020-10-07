using System;
using System.Collections.Generic;

namespace Domain
{
    public class Channel: BaseEntity
    {
        public Guid AccountOwnerId { get; set; }
        public Catalog CurrentCatalog { get; set; }
        public IEnumerable<Catalog> HistoryCatalogs { get; set; }
    }
}
