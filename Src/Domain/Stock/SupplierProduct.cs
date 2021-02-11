namespace Domain.Stock
{
    public class SupplierProduct : BaseEntity
    {
        public Supplier Supplier { get; private set; }
        public string Code { get; private set; }
        public ProductStock Product { get; private set; }
        protected SupplierProduct()
        {

        }

        public SupplierProduct(Supplier supplier, ProductStock product, string code)
        {
            Supplier = supplier;
            Product = product;
            Code = code;
        }
    }
}
