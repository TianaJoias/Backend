using System.Linq;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.AspNetCore.Endpoints;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebApi.Extensions
{
    public static class MetricsExtensions
    {
        public static IHostBuilder ConfigureOwnerMetrics(this IHostBuilder hostBuilder)
        {
            var Metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Build();

            return hostBuilder.ConfigureMetrics(Metrics)
                .UseMetrics(
                            options =>
                            {
                                options.EndpointOptions = endpointsOptions =>
                                {
                                    endpointsOptions.MetricsEndpointOutputFormatter = Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                                    endpointsOptions.MetricsTextEndpointOutputFormatter = Metrics.OutputMetricsFormatters.OfType<MetricsPrometheusProtobufOutputFormatter>().First();
                                };
                            })
                .ConfigureServices((context, services) =>
                {
                    services.Configure<MetricsEndpointsHostingOptions>(options =>
                    {
                        options.AllEndpointsPort = context.Configuration.GetValue("ManagementPort", 8081);
                    });
                    services.AddAppMetricsSystemMetricsCollector();
                    services.AddAppMetricsGcEventsMetricsCollector();
                });
        }
    }
}
