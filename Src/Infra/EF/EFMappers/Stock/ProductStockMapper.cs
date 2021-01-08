using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class ProductStockMapper : EntityMapper<ProductStock>
    {
        public override void Configure(EntityTypeBuilder<ProductStock> builder)
        {
            base.Configure(builder);
            builder.ToTable("ProductStock");
            builder.Property(x => x.ProductId);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.ReservedQuantity);
        }
    }
}
