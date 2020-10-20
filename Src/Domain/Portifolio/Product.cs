using System.Collections.Generic;

namespace Domain.Portifolio
{
    public class Product : BaseEntity
    {
        public string EAN { get; set; }
        public string Description { get; set; }
        public IList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();
        public string SKU { get; private set; }

        public void AddCategory(Tag tag)
        {
            Categories.Add(new ProductCategory(this, tag));
        }

        public void RemoveAllCategories()
        {
            Categories.Clear();
        }
    }
}
