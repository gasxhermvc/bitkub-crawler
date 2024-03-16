using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector.Connectors
{
    public class OracleConnector : DatabaseConnector
    {
        //public OracleConnector(ProviderFactory provider)
        //    : base(ProviderFactory.Oracle)
        //{

        //}

        //public OracleConnector(string connectionString)
        //    : base(connectionString, ProviderFactory.Oracle)
        //{
        //}

        public OracleConnector(ILogger logger, string connectionString, DataSource DataSource)
          : base(logger, connectionString, ProviderFactory.Oracle, DataSource)
        {
        }
    }
}
