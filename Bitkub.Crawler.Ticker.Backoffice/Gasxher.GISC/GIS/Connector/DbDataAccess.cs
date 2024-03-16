using GasxherGIS.GIS.Connector.Internal;
using GasxherGIS.GIS.Utility;
using GasxherGIS.Standards.Logging;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public class DbDataAccess : IDbDataAccess
    {
        public DbConfigProvider Options => _options.Value ?? throw new ArgumentNullException(nameof(Options));

        private readonly ILogger _logger;
        private readonly IOptions<DbConfigProvider> _options;

        public DbDataAccess(DatabaseLoggerHandler<DbDataAccess> logger
            , IOptions<DbConfigProvider> options)
        {
            _logger = logger;
            _options = options;
        }

        public IDatabaseConnector DbConnection() => this.CreateFactory(_logger, Options.DefaultDataSource);

        public IDatabaseConnector DbConnection(string dataSourceName)
        {
            var dbConnection = this.CreateFactory(_logger, dataSourceName);

            //=>Setting
            dbConnection.ConnectionTimeout = Options.ConnectionTimeout;
            dbConnection.CommandTimeout = Options.CommandTimeout;

            return dbConnection;
        }
    }
}
