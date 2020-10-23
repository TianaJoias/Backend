using System.Collections.Generic;
using Domain.Catalog;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Catalog
{
    internal class CatalogMapper : EntityMapper<Domain.Catalog.Catalog>
    {
        public override void Configure(EntityTypeBuilder<Domain.Catalog.Catalog> builder)
        {
            base.Configure(builder);
            builder.ToTable("Catalogs");
            builder.Property(it => it.ClosedAt);
            builder.Property(it => it.CreatedAt);
            builder.Property(it => it.TotalSold);
            builder.HasOne(it => it.Agent).WithMany();
            builder.HasMany(it => it.Items).WithOne().HasForeignKey("CatalogId").HasConstraintName("FK_CATALOG_ITEM").Metadata.PrincipalToDependent.SetPropertyAccessMode(PropertyAccessMode.Field);
            //builder.Property<IList<CatalogItem>>("_items").HasField("_items");
                //.FindNavigation("Items")
                //.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}
