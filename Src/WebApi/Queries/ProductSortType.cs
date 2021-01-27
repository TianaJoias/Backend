using Domain;
using GraphQL.Types;

namespace WebApi.Queries
{
    public class ProductSortType : InputObjectGraphType
    {
        public ProductSortType()
        {
            Name = "ProductSort";
            Field<EnumerationGraphType<Sort>>("Id");
            Field<EnumerationGraphType<Sort>>("BarCode");
            Field<EnumerationGraphType<Sort>>("Description");
        }
    }
}
