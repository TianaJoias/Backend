using System;
using System.Collections.Generic;

namespace Domain
{
    public class Catalog: BaseEntity
    {
        public IEnumerable<CatalogItem> Items { get; set; }
        public DateTime Opened { get; set; }
        public DateTime Closed { get; set; }
    }
}
