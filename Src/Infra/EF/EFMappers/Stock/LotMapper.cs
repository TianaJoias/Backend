using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers
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
            builder.Property(x => x.Date);
            builder.Property(x => x.SaleValue);
            builder.Property(x => x.CostValue);
            builder.HasMany(x => x.Suppliers).WithMany(x => x.Lots);
        }
    }
}
