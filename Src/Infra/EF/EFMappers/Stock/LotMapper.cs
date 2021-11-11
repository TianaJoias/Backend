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
            builder.OwnsOne(it => it.Prices);
            builder.OwnsOne(x => x.Infos);
            builder.OwnsOne(x => x.Reserves); 
            builder.HasMany(x => x.Suppliers).WithMany(x => x.Lots);
        }
    }
}
