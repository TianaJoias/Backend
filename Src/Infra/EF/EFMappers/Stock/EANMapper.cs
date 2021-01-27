using Domain.Stock;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Stock
{
    internal class EANMapper : EntityMapper<EAN>
    {
        public override void Configure(EntityTypeBuilder<EAN> builder)
        {
            base.Configure(builder);

            builder.ToTable("EAN");
            builder.Property(it => it.LastCode);
        }
    }
}
