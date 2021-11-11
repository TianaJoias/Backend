using Domain.Products.Write;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.Products.Mappers
{
    internal class CategoryMapper : EntityMapper<Category>
    {
        public override void Configure(EntityTypeBuilder<Category> builder)
        {
            base.Configure(builder);
            builder.ToTable("Categories");
            builder.Property(it => it.Title);
            builder.Property(it => it.UpdateAt);
            builder.Property(it => it.CreateAt);
        }
    }
}
