using Domain.Account;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infra.EF.EFMappers.Account
{
    internal class IdentityProviderMapper : EntityMapper<IdentityProvider>
    {
        public override void Configure(EntityTypeBuilder<IdentityProvider> builder)
        {

            base.Configure(builder);
            builder.ToTable("IdentityProviders");
            builder.Property(x => x.SubjectId);
            builder.Property(x => x.Provider);
        }
    }
}
