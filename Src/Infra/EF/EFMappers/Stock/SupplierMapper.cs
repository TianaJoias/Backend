using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers
{
    internal class SupplierMapper : EntityMapper<Supplier>
    {
        public override void Configure(EntityTypeBuilder<Supplier> builder)
        {
            base.Configure(builder);
            builder.ToTable("Supplier");
            builder.Property(x => x.Description);
            builder.Property(x => x.Name);
        }
    }
}
