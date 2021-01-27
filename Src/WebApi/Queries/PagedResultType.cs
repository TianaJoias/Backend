using Domain;
using GraphQL.Types;

namespace WebApi.Queries
{
    public class PagedResultType<T, TType> : ObjectGraphType<PagedResult<T>>
        where TType : IGraphType
    {
        public PagedResultType()
        {
            Name = "Page";
            Field(x => x.CurrentPage).Description("Current page number");
            Field(x => x.PageSize).Description("Total of records per Page");
            Field(x => x.PageCount).Description("Total of pages");
            Field(x => x.RowCount).Description("Total of records");
            Field<ListGraphType<TType>>("Records", description: "List of records");
        }
    }
}
