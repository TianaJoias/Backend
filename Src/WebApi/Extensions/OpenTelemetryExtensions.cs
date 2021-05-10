using System.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OpenTelemetry;
using OpenTelemetry.Logs;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace WebApi.Extensions
{
    public static class OpenTelemetryExtensions
    {
        public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, IConfiguration configuration)
        {
            var resource = ResourceBuilder.CreateDefault().AddService("BackofficeApi");
            services.AddOpenTelemetryTracing(
                (builder) =>
                {
                    builder
                     .SetResourceBuilder(resource)
                      .AddAspNetCoreInstrumentation()
                      .AddSqlClientInstrumentation()
                      .AddJaegerExporter(options =>
                      {
                          options.AgentHost = configuration.GetSection("Jaeger")?.GetValue<string>("Host") ?? string.Empty;
                          options.AgentPort = configuration.GetSection("Jaeger")?.GetValue<int>("Port") ?? 0;
                          options.ExportProcessorType = ExportProcessorType.Batch;
                          options.BatchExportProcessorOptions = new BatchExportProcessorOptions<Activity>()
                          {
                              MaxQueueSize = 2048,
                              ScheduledDelayMilliseconds = 5000,
                              ExporterTimeoutMilliseconds = 30000,
                              MaxExportBatchSize = 512,
                          };
                      });
                });

            services.AddLogging(builder =>
            {
                builder.AddOpenTelemetry(options =>
                {
                    options.SetResourceBuilder(resource).AddConsoleExporter();
                });
            });
            return services;
        }

    }
}
