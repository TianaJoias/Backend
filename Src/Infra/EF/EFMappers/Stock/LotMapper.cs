using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class LotMapper : EntityMapper<Lot>
    {
        public override void Configure(EntityTypeBuilder<Lot> builder)
        {
            base.Configure(builder);

            builder.ToTable("Lots");
            builder.Property(it => it.ProductId);
            builder.Property(x => x.Number);
            builder.Property(x => x.Weight);
            builder.Property(x => x.Quantity);
            builder.Property(x => x.ReservedQuantity);
            builder.Property(x => x.CreatedAt);
            builder.Property(x => x.SalePrice);
            builder.Property(x => x.CostPrice);
            builder.HasMany(x => x.Suppliers).WithMany(x => x.Lots);
        }
    }
}
