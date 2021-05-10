using System;
using Microsoft.Extensions.DependencyInjection;
using ZiggyCreatures.Caching.Fusion;

namespace WebApi.Extensions
{
    public static class CacheExtensions
    {
        public static IServiceCollection AddCache(this IServiceCollection services)
        {

            // REGISTER REDIS AS A DISTRIBUTED CACHE
            //services.AddStackExchangeRedisCache(options =>
            //{
            //    options.Configuration = "YOUR CONNECTION STRING HERE";
            //});

            services.AddMemoryCache();

            // REGISTER THE FUSION CACHE SERIALIZER
            services.AddFusionCacheSystemTextJsonSerializer();

            // REGISTER FUSION CACHE
            services.AddFusionCache(options =>
            {
                options.DefaultEntryOptions = new FusionCacheEntryOptions
                {
                    // CACHE DURATION
                    Duration = TimeSpan.FromMinutes(1),

                    // FAIL-SAFE OPTIONS
                    IsFailSafeEnabled = true,
                    FailSafeMaxDuration = TimeSpan.FromHours(2),
                    FailSafeThrottleDuration = TimeSpan.FromSeconds(30)
                };
            });
            return services;
        }
    }
}
