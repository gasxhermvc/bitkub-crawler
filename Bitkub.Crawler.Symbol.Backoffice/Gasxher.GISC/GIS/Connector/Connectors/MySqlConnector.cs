using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector.Connectors
{
    public class MySqlConnector : DatabaseConnector
    {
        //public MySqlConnector()
        //   : base(ProviderFactory.MySQL)
        //{
        //}

        //public MySqlConnector(string connectionString)
        //    : base(connectionString, ProviderFactory.MySQL)
        //{
        //}

        public MySqlConnector(ILogger logger, string connectionString, DataSource DataSource)
           : base(logger, connectionString, ProviderFactory.MySQL, DataSource)
        {
        }
    }
}
