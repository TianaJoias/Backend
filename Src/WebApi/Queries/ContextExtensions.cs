using System;
using System.Threading.Tasks;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Queries
{
    public static class ContextExtensions
    {
        public static async Task<TReturn> RunScopedAsync<TSource, TReturn>(
            this IResolveFieldContext<TSource> context,
            Func<IResolveFieldContext<TSource>, IServiceProvider, Task<TReturn>> func)
        {
            using var scope = context.RequestServices.CreateScope();
                return await func(context, scope.ServiceProvider);
        }
    }
}
