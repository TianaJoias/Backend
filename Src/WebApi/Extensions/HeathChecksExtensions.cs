using Infra.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Extensions
{
    public static class AddHealthChecksExtensions
    {
        public static IServiceCollection AddHealthChecksCustom(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TianaJoiasContextDB>(tags: new[] { "Default" })
                .AddProcessAllocatedMemoryHealthCheck(200, tags: new[] { "Default" });
            return services;
        }
        public static void UseHealthChecks(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHealthChecks("/health/liveness", new
                HealthCheckOptions()
            {
                Predicate = (_) => false
            });

            endpoints.MapHealthChecks("/health/readiness", new
                 HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("Default")
            });
        }

    }
}
