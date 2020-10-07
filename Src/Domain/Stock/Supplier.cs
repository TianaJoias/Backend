using System.Collections.Generic;

namespace Domain
{
    public class Supplier : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public virtual IList<Lot> Lots { get; set; }
    }
}
