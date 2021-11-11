using System.Collections.Generic;
using Domain.Products.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Products.Mappers
{
    internal class CollectionMapper : EntityMapper<Collection>
    {
        public override void Configure(EntityTypeBuilder<Collection> builder)
        {
            base.Configure(builder);
            builder.ToTable("Collections");
            builder.Property(it => it.Title);
            builder.Property(it => it.UpdateAt);
            builder.Property(it => it.CreateAt);
            builder.Ignore(it => it.Images);

            builder
                .HasMany(p => p.Products)
                .WithMany(p => p.Collections)
                .UsingEntity<Dictionary<string, object>>(
                    "CollectionProducts",
                    j => j
                        .HasOne<Product>()
                        .WithMany()
                        .HasForeignKey("ProductId")
                        .HasConstraintName("FK_CollectionProducts_Product_ProductId")
                        .OnDelete(DeleteBehavior.Cascade),
                    j => j
                        .HasOne<Collection>()
                        .WithMany()
                        .HasForeignKey("CollectionId")
                        .HasConstraintName("FK_CollectionProducts_Collection_CollectionId")
                        .OnDelete(DeleteBehavior.ClientCascade));
        }
    }
}
