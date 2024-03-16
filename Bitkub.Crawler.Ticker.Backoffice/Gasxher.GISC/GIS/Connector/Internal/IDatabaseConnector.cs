using GasxherGIS.GIS.Utility;
using System;
using System.Collections.Generic;
using System.Data;

namespace GasxherGIS.GIS.Connector.Internal
{
    public interface IDatabaseConnector
    {
        string ConnectionString
        {
            get;
            set;
        }
        int ConnectionTimeout
        {
            get;
            set;
        }
        int CommandTimeout
        {
            get;
            set;
        }
        ProviderFactory Provider
        {
            get;
            set;
        }
        string AssemblyVersion
        {
            get;
            set;
        }
        System.Globalization.CultureInfo CultureInfo
        {
            get;
            set;
        }
        string DateTimeFormat
        {
            get;
            set;
        }
        double TimeZoneOffset
        {
            get;
            set;
        }

        IDbConnection DbConnection { get; }

        DataSource DataSource { get; }

        QueryResult BulkCopy(string destinationTableName, DataTable dataTable, int copyBatchSize = 5000, int copyTimeout = 3600);

        QueryResult BulkCopy(string destinationTableName, DataTable dataTable, Dictionary<string, string> columnMapping, int copyBatchSize = 5000, int copyTimeout = 3600);

        /// <summary>
        /// Default call store prodecure
        /// </summary> 
        /// <param name="queryParameter"></param>
        /// <returns></returns>
        QueryResult ExecuteProcedure(QueryParameter queryParameter);

        /// <summary>
        /// Custom for send Datatable to store prodecure
        /// </summary>
        /// <param name="spName"></param>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        QueryResult ExecuteProcedure(string spName, string PI, DataTable dataTable);


        /// <summary>
        /// Single Execute
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        QueryResult ExecuteRawSQLCommand(string sqlCommand, bool useTrasaction = false);


        /// <summary>
        /// Single Execute
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns></returns>
        QueryResult ExecuteRawSQLQuery(string sqlCommand);


        /// <summary>
        /// Custom for send sql command to execute
        /// </summary>
        /// <param name="dbConnectionDelegate"></param>
        /// <returns></returns>
        QueryResult ExecuteSQLCommand(Func<IDbConnection, IDbTransaction, QueryResult> dbConnectionDelegate, bool useTrasaction = false);
    }
}
