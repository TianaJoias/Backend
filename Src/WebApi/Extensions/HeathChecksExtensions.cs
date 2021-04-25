using Infra.EF;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace WebApi.Extensions
{
    public static class AddHealthChecksExtensions
    {
        public static IServiceCollection AddHealthChecksCustom(this IServiceCollection services)
        {
            services.AddHealthChecks()
                .AddDbContextCheck<TianaJoiasContextDB>(tags: new[] { "Default" }, failureStatus: HealthStatus.Unhealthy)
                .AddProcessAllocatedMemoryHealthCheck(200, failureStatus: HealthStatus.Degraded, tags: new[] { "Default" });
            return services;
        }
        public static void UseHealthChecks(this IEndpointRouteBuilder endpoints, IConfiguration configuration)
        {
            endpoints.MapHealthChecks("/health")
                .RequireHost($"*:{configuration.GetValue("ManagementPort", 8081)}");

            endpoints.MapHealthChecks("/health/liveness", new
                HealthCheckOptions()
            {
                Predicate = (_) => false
            });

            endpoints.MapHealthChecks("/health/readiness", new
                 HealthCheckOptions()
            {
                Predicate = (check) => check.Tags.Contains("Default"),
                AllowCachingResponses = false,
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Degraded] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                }
            });
        }

    }
}
