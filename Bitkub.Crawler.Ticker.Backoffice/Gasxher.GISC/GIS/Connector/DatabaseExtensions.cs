using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasxherGIS.GIS.Connector.Internal;
using GasxherGIS.GIS.Connector.Connectors;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector
{
    internal static class DatabaseExtensions
    {
        public static IDatabaseConnector CreateFactory(this DbDataAccess dataAccess, ILogger logger) => dataAccess.CreateFactory(logger, null);

        public static IDatabaseConnector CreateFactory(this DbDataAccess dataAccess, ILogger logger, string dataSourceName)
        {
            var dataSource = dataAccess.Options.DataSource[dataSourceName ?? DbDefaults.DefaultDataSourceKey];

            if (dataSource == null)
            {
                throw new InvalidOperationException($"ไม่พบ DataSource {dataSourceName ?? DbDefaults.DefaultDataSourceKey} บน Configuration");
            }

            dataSource.ConnectionString = dataSource.CleanConnectionString(dataAccess);

            return dataSource.Provider switch
            {
                ProviderFactory.MSSQL => new SqlConnector(logger, dataSource.ConnectionString, dataSource),
                ProviderFactory.MySQL => new MySqlConnector(logger, dataSource.ConnectionString, dataSource),
                ProviderFactory.Oracle => new OracleConnector(logger, dataSource.ConnectionString, dataSource),
                ProviderFactory.PostgreSQL => new PostgreSqlConnector(logger, dataSource.ConnectionString, dataSource),
                _ => throw new InvalidOperationException($"ไม่พบ Driver บริการสำหรับการเชื่อมต่อด้วย {dataSource.Provider}")
            };
        }

        public static string CleanConnectionString(this DataSource dataSource, DbDataAccess dataAccess)
        {
            dataSource.ConnectionString = dataSource.ConnectionString.Trim(';');
            dataSource.ConnectionString = string.Format("{0}; User Id={1};Password={2};Connection Timeout={3}"
                , dataSource.ConnectionString, dataSource.UserId, dataSource.Password
                , dataAccess.Options.ConnectionTimeout);

            return dataSource.ConnectionString;
        }
    }
}
