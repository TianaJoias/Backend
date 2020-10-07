using System.Collections.Generic;

namespace Domain
{
    public class Product : BaseEntity
    {
        public string EAN { get; set; }
        public string Description { get; set; }
        public IList<ProductCategory> Categories { get; set; } = new List<ProductCategory>();

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
