using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Catalog
{
    internal class CatalogItemsMapper : EntityMapper<CatalogItem>
    {
        public override void Configure(EntityTypeBuilder<CatalogItem> builder)
        {

            base.Configure(builder);
            builder.ToTable("CatalogItems");
            builder.Property(it => it.EAN);
            builder.Property(it => it.Enabled);
            builder.Property(it => it.LongDescription);
            builder.Property(it => it.LotId);
            builder.Property(it => it.Price);
            builder.Property(it => it.ProdutoId);
            builder.Property(it => it.InitialQuantity);
            builder.Property(it => it.CurrentQuantity);
            builder.Property(it => it.ShortDescription);
            builder.Property(it => it.SKU);
            builder.Property(it => it.ValueSold);
            builder.Property(it => it.QuantitySold);
            builder.Property(it => it.Thumbnail).HasJsonConversion();
        }
    }
}
