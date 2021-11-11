using Application.Products.Repositories;
using Domain.Products.Write;

namespace Infra.Products.Repositories
{
    public class ProductRepository : RepositoryBase<Product>, IProductRepository
    {
        public ProductRepository(ProductContextDB context) : base(context)
        { }
    }
    public class VariantRepository : RepositoryBase<Variant>, IVariantRepository
    {
        public VariantRepository(ProductContextDB context) : base(context)
        { }
    }
    public class CollectionRepository : RepositoryBase<Collection>, ICollectionRepository
    {
        public CollectionRepository(ProductContextDB context) : base(context)
        { }
    }
    public class CategoryRepository : RepositoryBase<Category>, ICategoryRepository
    {
        public CategoryRepository(ProductContextDB context) : base(context)
        { }
    }
}
