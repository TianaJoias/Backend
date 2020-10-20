using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Catalog
{
    public interface ICatalogRepository : IRepository<Catalog>
    {
    }
    public interface IChannelRepository : IRepository<Channel>
    {
    }
}
