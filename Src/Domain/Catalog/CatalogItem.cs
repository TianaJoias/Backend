using System;
using System.Collections.Generic;

namespace Domain
{
    public class CatalogItem : BaseEntity
    {
        public Guid LotId { get; set; }
        public Guid ProdutoId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string SKU { get; set; }
        public string EAN { get; set; }
        public string LongDescription { get; set; }
        public string ShortDescription { get; set; }
        public IList<string> Thumbnail { get; set; }
        public bool Enabled { get; set; }
    }
}
