using Application;
using System.Collections.Generic;

namespace WebApi.Controllers
{
    public class FilterPaged
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public Dictionary<string, SortDirection> Sort { get; set; }
    }

}
