using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Domain;
using Domain.Specification;

namespace Application.Common
{
    public static class SpecifcationBuilderExtensions
    {
        private static bool IsSimpleType(Type type)
        {
            var types = new Type[] {
            typeof(string),
            typeof(decimal),
            typeof(DateTime),
            typeof(DateTimeOffset),
            typeof(TimeSpan),
            typeof(Guid)
                };
            return
                type.IsPrimitive ||
               types.Contains(type) ||
                type.IsEnum ||
                Convert.GetTypeCode(type) != TypeCode.Object ||
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>) && IsSimpleType(type.GetGenericArguments()[0]))
                ;
        }
        public static SpecifcationBuilder<T> Sort<T>(this SpecifcationBuilder<T> it, Dictionary<string, SortDirection> props)
        {
            foreach (var item in props)
            {
                var propertyinfo = typeof(T).GetProperty(item.Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (propertyinfo is null || IsSimpleType(propertyinfo.PropertyType))
                    throw new($"Property '{item.Key}' not exists on '{nameof(T)}'");
                ParameterExpression argParam = Expression.Parameter(typeof(T), "it");
                Expression nameProperty = Expression.Property(argParam, propertyinfo);
                Expression conversion = Expression.Convert(nameProperty, typeof(object));
                var expression = Expression.Lambda<Func<T, object>>(conversion, argParam);

                if (item.Value == SortDirection.Asc)
                    it.WithOrderBy(expression);
                else
                    it.WithOrderByDescending(expression);
            }

            return it;

        }
    }
}
