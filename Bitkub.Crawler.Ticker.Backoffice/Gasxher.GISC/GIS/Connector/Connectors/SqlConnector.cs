using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector.Connectors
{
    public class SqlConnector : DatabaseConnector
    {
        //public SqlConnector()
        //    : base(ProviderFactory.MSSQL)
        //{
        //}
        //public SqlConnector(string connectionString)
        //    : base(connectionString, ProviderFactory.MSSQL)
        //{
        //}
        public SqlConnector(ILogger logger, string connectionString, DataSource DataSource)
           : base(logger, connectionString, ProviderFactory.MSSQL, DataSource)
        {
        }
    }
}
