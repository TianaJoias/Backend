using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class ProductStockMapper : EntityMapper<StockUnit>
    {
        public override void Configure(EntityTypeBuilder<StockUnit> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductStock");
            builder.Property(x => x.Quantity);
            builder.Property(x => x.ReservedQuantity);
        }
    }

    internal class SupplierProductMapper : EntityMapper<ProductSupplier>
    {
        public override void Configure(EntityTypeBuilder<ProductSupplier> builder)
        {
            base.Configure(builder);
            builder.ToTable("SupplierProduct");
            builder.Property(x => x.Code);
            builder.HasOne(x => x.Supplier);
            builder.HasOne(x => x.Product);
        }
    }
}
