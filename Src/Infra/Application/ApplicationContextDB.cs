using Microsoft.EntityFrameworkCore;
using Infra.Products.Mappers;
using Microsoft.Extensions.Logging;
using System;
using Infra.Application.Idempotency;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using BuildingBlocks.EventBus;

namespace Infra.Application
{

    /// <summary>
    /// https://blog.tekspace.io/code-first-multiple-db-context-migration/
    /// </summary>
    public sealed class ApplicationContextDB : DbContext
    {
        private readonly ILogger<ApplicationContextDB> _logger;

        public ApplicationContextDB(DbContextOptions<ApplicationContextDB> options, ILogger<ApplicationContextDB> logger) : base(options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(IntegrationEventMapper).Assembly);
        }
    }

    internal class ClientRequestMapper : IEntityTypeConfiguration<ClientRequest>
    {

        public void Configure(EntityTypeBuilder<ClientRequest> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(X => X.Name);
            builder.Property(X => X.Time);
        }
    }

    internal class IntegrationEventMapper : IEntityTypeConfiguration<IntegrationEventLogEntry>
    {

        public void Configure(EntityTypeBuilder<IntegrationEventLogEntry> builder)
        {
            builder.ToTable("IntegrationEventLog");

            builder.HasKey(e => e.EventId);

            builder.Property(e => e.EventId)
                .IsRequired();

            builder.Property(e => e.Content)
                .IsRequired();

            builder.Property(e => e.CreationTime)
                .IsRequired();

            builder.Property(e => e.State)
                .IsRequired();

            builder.Property(e => e.TimesSent)
                .IsRequired();

            builder.Property(e => e.EventTypeName)
                .IsRequired();
        }
    }
}
