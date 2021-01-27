using System.Collections.Generic;

namespace Domain.Portifolio
{
    public class Tag : BaseEntity
    {
        public string Name { get; set; }
        public IList<Product> Products { get; set; }
    }
}
