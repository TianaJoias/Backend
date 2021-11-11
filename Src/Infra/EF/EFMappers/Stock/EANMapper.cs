using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class EANMapper : EntityMapper<Configuration>
    {
        public override void Configure(EntityTypeBuilder<Configuration> builder)
        {
            base.Configure(builder);

            builder.ToTable("EAN");
            builder.Property(it => it.LastCode);
        }
    }
}
