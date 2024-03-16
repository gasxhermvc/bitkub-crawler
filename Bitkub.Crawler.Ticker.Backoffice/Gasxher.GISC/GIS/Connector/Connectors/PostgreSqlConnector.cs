using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector.Connectors
{
    public class PostgreSqlConnector : DatabaseConnector
    {
        //public PostgreSqlConnector()
        //  : base(ProviderFactory.PostgreSQL)
        //{
        //}
        //public PostgreSqlConnector(string connectionString)
        //    : base(connectionString, ProviderFactory.PostgreSQL)
        //{
        //}

        public PostgreSqlConnector(ILogger logger, string connectionString, DataSource DataSource)
            : base(logger, connectionString, ProviderFactory.PostgreSQL, DataSource)
        {
        }
    }
}
