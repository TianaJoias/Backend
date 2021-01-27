using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Domain;
using Domain.Portifolio;
using GraphQL;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace WebApi.Queries
{
    //https://hasura.io/docs/1.0/graphql/core/queries/query-filters.html#using-multiple-filters-in-the-same-query-and-or
    public static class ComparisionOperators
    {
        public const string EQUAL = "_eq";
        public const string NOT_EQUAL = "_neq";
        public const string CONTAINS = "_contains";
        public const string NOT_CONTAINS = "_ncontains";
        public const string GREATER_THAN = "_gt";
        public const string GREATER_THAN_OR_EQUALS = "_gte";
        public const string LESS_THAN = "_lt";
        public const string LESS_THAN_OR_EQUALS = "_lte";
        public const string NOT = "_not";
        public const string AND = "_and";
        public const string OR = "_or";
        public const string IN = "_in";
        public const string NOT_IN = "_nin";
    }
    public class StringComparisonOperationType : InputObjectGraphType
    {
        public StringComparisonOperationType()
        {
            Field<StringGraphType>(ComparisionOperators.EQUAL);
            Field<StringGraphType>(ComparisionOperators.NOT_EQUAL);
            Field<StringGraphType>(ComparisionOperators.CONTAINS);
            Field<StringGraphType>(ComparisionOperators.NOT_CONTAINS);
            Field<ListGraphType<StringGraphType>>(ComparisionOperators.IN);
            Field<ListGraphType<StringGraphType>>(ComparisionOperators.NOT_IN);
        }
    }

    public class ArrayComparisonOperationType : InputObjectGraphType
    {
        public ArrayComparisonOperationType()
        {
            Field<ListGraphType<StringGraphType>>(ComparisionOperators.NOT_CONTAINS);
            Field<ListGraphType<StringGraphType>>(ComparisionOperators.CONTAINS);
        }
    }
    public class NumberComparisonOperationType : InputObjectGraphType
    {
        public NumberComparisonOperationType()
        {
            Field<DecimalGraphType>(ComparisionOperators.GREATER_THAN);
            Field<DecimalGraphType>(ComparisionOperators.GREATER_THAN_OR_EQUALS);
            Field<DecimalGraphType>(ComparisionOperators.LESS_THAN);
            Field<DecimalGraphType>(ComparisionOperators.LESS_THAN_OR_EQUALS);
            Field<ListGraphType<DecimalGraphType>>(ComparisionOperators.IN);
            Field<ListGraphType<DecimalGraphType>>(ComparisionOperators.NOT_IN);
        }
    }
    public abstract class BaseWhereClauseType<TD> : InputObjectGraphType where TD : class
    {
        public BaseWhereClauseType()
        {
            Field(GetType(), ComparisionOperators.NOT);
            Field(GetType(), ComparisionOperators.AND);
            Field(GetType(), ComparisionOperators.OR);
        }
        public void FieldString(Expression<Func<TD, object>> expression)
        {
            Field<StringComparisonOperationType>(expression.GetPropertyName());
        }
        public void FieldNumber(Expression<Func<TD, object>> expression)
        {
            Field<NumberComparisonOperationType>(expression.GetPropertyName());
        }

        public void FieldArray(Expression<Func<TD, object>> expression)
        {
            Field<ArrayComparisonOperationType>(expression.GetPropertyName());
        }
    }
    public static class ExpressionExtension
    {
        public static string GetPropertyName<TD>(this Expression<Func<TD, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo.Name;
            }

            return null;
        }
    }
    public class GraphQLWhereClauseHelper<T>
    {

        private static Dictionary<string, Func<string, object, Expression<Func<T, bool>>>> aasd = new()
        {
            { ComparisionOperators.EQUAL, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"{key} == @0", value) },
            { ComparisionOperators.CONTAINS, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"{key}.Contains(@0)", value) },
            { ComparisionOperators.NOT_EQUAL, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"{key} != @0", value) },
            { ComparisionOperators.NOT_CONTAINS, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!{key}.Contains(@0)", value) },
            { ComparisionOperators.GREATER_THAN, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!{key} > @0", value) },
            { ComparisionOperators.GREATER_THAN_OR_EQUALS, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!{key} >=@0", value) },
            { ComparisionOperators.LESS_THAN, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!{key} < @0", value) },
            { ComparisionOperators.LESS_THAN_OR_EQUALS, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!{key} <= @0", value) },
            { ComparisionOperators.IN, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"@0.Contains({key})", value) },
            { ComparisionOperators.NOT_IN, (key, value) => DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!@0.Contains({key})", value) }
        };
        private static Dictionary<string, Func<Dictionary<string, object>, Expression<Func<T, bool>>>> _oth = new()
        {
            { "_not", BuildNotExpression },
            { "_and", BuildAndExpression },
            { "_or", BuildOrExpression }
        };
        public static Expression<Func<T, bool>> ParseWhereToLambda(Dictionary<string, object> whereClause, string teste = "OR")
        {
            if (whereClause is null || whereClause.Count == 0)
                return p => true;
            var clauses = GetClauses(whereClause);
            var query = String.Join(" OR ", clauses.Select((value, index) => $"@{index}(it)"));
            return DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, query, clauses.ToArray());
        }

        private static List<Expression<Func<T, bool>>> GetClauses(Dictionary<string, object> whereClause)
        {
            var x = whereClause.ToDictionary(i => i.Key, i => (Dictionary<string, object>)i.Value);
            List<Expression<Func<T, bool>>> clauses = new();
            foreach (var item in x)
            {
                clauses.Add(GetClause(item));
            }

            return clauses;
        }

        private static Expression<Func<T, bool>> GetClause(KeyValuePair<string, Dictionary<string, object>> item)
        {
            if (_oth.ContainsKey(item.Key))
            {
                return _oth[item.Key](item.Value);
            }
            else
            {
                var ba = item.Value.First();
                return aasd[ba.Key](item.Key, ba.Value);
            }
        }

        public static Expression<Func<T, bool>> BuildAndExpression(Dictionary<string, object> items)
        {
            var x = GetClauses(items);
            var query = String.Join(" AND ", x.Select((value, index) => $"@{index}(it)"));
            return DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, query, x.ToArray());
        }
        public static Expression<Func<T, bool>> BuildOrExpression(Dictionary<string, object> items)
        {
            var x = GetClauses(items);
            var query = String.Join(" OR ", x.Select((value, index) => $"@{index}(it)"));
            return DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, query, x.ToArray());
        }
        public static Expression<Func<T, bool>> BuildNotExpression(Dictionary<string, object> items)
        {
            var x = GetClauses(items);
            return DynamicExpressionParser.ParseLambda<T, bool>(ParsingConfig.Default, false, $"!(@0(it))", x.First());
        }
    }
    public class ProductWhereClauseType : BaseWhereClauseType<Product>
    {
        public ProductWhereClauseType()
        {
            FieldString(it => it.SKU);
            FieldString(it => it.Description);
            FieldString(it => it.Id);
            //  FieldArray(it=> it.Categories);
        }
    }
    public class ProductQuery : ObjectGraphType<object>
    {
        public string ToTitleCase(string str)
        {
            return str.First().ToString().ToUpper() + str.Substring(1);
        }
        public ProductQuery()
        {
            Field<PagedResultType<Product, ProductType>>("Products", description: "Query of products",
                 arguments: new QueryArguments(new QueryArgument[]
                 {
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="page", DefaultValue = 0},
                    new QueryArgument<NonNullGraphType<IntGraphType>>{Name="pageSize", DefaultValue = 10},
                    new QueryArgument<ProductSortType>{Name="sortBy", DefaultValue = new Dictionary<string, object> { { "Description", Sort.Asc } }  },
                    new QueryArgument<ProductWhereClauseType>{ Name="where"  }
                 }), resolve: context => context.RunScopedAsync(Run));
        }

        private async Task<object> Run(IResolveFieldContext<object> context, IServiceProvider serviceProvider)
        {
            var repository = serviceProvider.GetService<IProductRepository>();
            var page = context.GetArgument<int>("page");
            var pageSize = context.GetArgument<int>("pageSize");
            var sortBy = context.GetArgument<Dictionary<string, object>>("sortBy").ToDictionary(entry => entry.Key, entry => (Sort)entry.Value);
            var where = context.GetArgument<Dictionary<string, object>>("where");
            var x = await repository.GetPaged(GraphQLWhereClauseHelper<Product>.ParseWhereToLambda(where), page, pageSize, sortBy);
            return x;
        }

    }
}
