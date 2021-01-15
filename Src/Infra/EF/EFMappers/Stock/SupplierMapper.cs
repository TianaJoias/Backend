using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class SupplierMapper : EntityMapper<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            base.Configure(builder);
            builder.ToTable("Suppliers");
            builder.Property(x => x.Description);
            builder.Property(x => x.Name);
        }
    }
}
