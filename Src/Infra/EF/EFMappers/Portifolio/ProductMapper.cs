using Domain;
using Domain.Portifolio;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Portifolio
{
    internal class ProductMapper : EntityMapper<Product>
    {
        public override void Configure(EntityTypeBuilder<Product> builder)
        {
            base.Configure(builder);
            builder.ToTable("Products");
            builder.Property(x => x.SKU);
            builder.Property(x => x.Description);
            builder.HasMany(x => x.Categories).WithOne(it => it.Product);
        }
    }
}
