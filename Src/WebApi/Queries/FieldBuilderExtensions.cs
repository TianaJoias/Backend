using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Builders;

namespace WebApi.Queries
{
    public static class FieldBuilderExtensions
    {
        public static FieldBuilder<TSource, TReturn> ResolveScopedAsync<TSource, TReturn>(
            this FieldBuilder<TSource, TReturn> builder,
            Func<IResolveFieldContext<TSource>, IServiceProvider, Task<TReturn>> func)
        {
            return builder.ResolveAsync(context => context.RunScopedAsync(func));
        }
    }
}
