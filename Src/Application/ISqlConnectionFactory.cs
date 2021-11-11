using System.Data;

namespace Application
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();
    }
}
