using GasxherGIS.GIS.Connector.Internal;
using GasxherGIS.GIS.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public partial class DatabaseConnector : IDatabaseConnector
    {
        /// <summary>
        /// Support MSSQL Server Only
        /// </summary>
        /// <param name="destinationTableName"></param>
        /// <param name="dataTable"></param>
        public QueryResult BulkCopy(string destinationTableName, DataTable dataTable, int copyBatchSize = 5000, int copyTimeout = 3600)
        {
            QueryResult queryResult = null;
            IDbTransaction transaction = null;
            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                    _logger.LogInformation("Connection Open");
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
                _logger.LogInformation("Connection Open");
            }


            try
            {
                transaction = this.DbConnection.BeginTransaction();

                using (SqlBulkCopy copy = new SqlBulkCopy(this.DbConnection.Database))
                {
                    copy.DestinationTableName = destinationTableName;
                    copy.BatchSize = copyBatchSize;
                    copy.BulkCopyTimeout = copyTimeout; //=>1hr

                    //=>Mapping columns
                    copy.WriteToServer(dataTable);

                    transaction.Commit();
                    copy.Close();
                    _logger.LogInformation("Execute success");
                }

                queryResult = queryResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);

                if (transaction != null)
                {
                    try
                    {
                        _logger.LogInformation("Start rollback");
                        transaction.Rollback();
                        _logger.LogInformation("Rollback success");
                    }
                    catch (Exception rollback)
                    {
                        queryResult = new QueryResult(rollback);
                    }
                }
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                    _logger.LogInformation("Connection Close");
                }
            }

            return queryResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="destinationTableName"></param>
        /// <param name="dataTable"></param>
        /// <param name="columnMapping">key = source, value = destination</param>
        /// <param name="copyBatchSize"></param>
        /// <param name="copyTimeout"></param>
        public QueryResult BulkCopy(string destinationTableName, DataTable dataTable, Dictionary<string, string> columnMapping, int copyBatchSize = 5000, int copyTimeout = 3600)
        {
            QueryResult queryResult = null;
            IDbTransaction transaction = null;

            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
            }


            try
            {
                transaction = this.DbConnection.BeginTransaction();

                using (SqlBulkCopy copy = new SqlBulkCopy(this.DbConnection.Database))
                {
                    copy.DestinationTableName = destinationTableName;
                    copy.BatchSize = copyBatchSize;
                    copy.BulkCopyTimeout = copyTimeout; //=>1hr

                    //=>Mapping columns
                    foreach (var key in columnMapping.Keys)
                    {
                        copy.ColumnMappings.Add(key, columnMapping[key]);
                    }

                    copy.WriteToServer(dataTable);

                    transaction.Commit();
                    copy.Close();
                }

                queryResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);

                if (transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollback)
                    {
                        queryResult = new QueryResult(rollback);
                    }
                }
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return queryResult;
        }

        public QueryResult ExecuteProcedure(QueryParameter queryParameter)
        {
            QueryResult queryResult = null;
            string storeName = string.Empty;
            try
            {
                if (queryParameter["SP"] != null)
                {
                    storeName = queryParameter["SP"].ToString();
                    queryParameter.Remove("SP");
                    queryResult = this.ExecuteStoredProcedure(storeName, queryParameter);

                }
                else
                {
                    throw new Exception("ไม่ได้ระบุชื่อ store มาที่ parameter ชื่อ 'SP'");
                }

            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);
            }

            return queryResult;
        }

        public QueryResult ExecuteProcedure(string spName, string PI, DataTable dataTable)
        {
            QueryResult queryResult = null;
            string storeName = string.Empty;
            //IDbTransaction transaction = null;

            //=>Check parameter
            try
            {
                if (string.IsNullOrEmpty(spName))
                {
                    throw new ArgumentNullException(nameof(spName));
                }

                if (dataTable == null)
                {
                    throw new ArgumentNullException(nameof(dataTable));
                }
            }
            catch (Exception ex)
            {
                return new QueryResult(ex);
            }

            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
            }

            try
            {
                var command = this.CreateCommand(spName);
                command.CommandType = System.Data.CommandType.StoredProcedure;

                SqlParameter _PI = new SqlParameter();
                _PI.ParameterName = $"@{PI}";
                _PI.SqlDbType = SqlDbType.Structured;
                _PI.Value = dataTable;
                command.Parameters.Add(_PI);

                command.CreatePO();
                command.ExecuteReader();
                queryResult = queryResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);
            }
            finally
            {

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return queryResult;
        }

        /// <summary>
        /// Single Execute
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns>Success or Fail</returns>
        public QueryResult ExecuteRawSQLCommand(string sqlCommand, bool useTrasaction = false)
        {
            QueryResult queryResult = null;
            IDbTransaction transaction = null;

            //=>Check parameter
            try
            {
                if (string.IsNullOrEmpty(sqlCommand))
                {
                    throw new ArgumentNullException(nameof(sqlCommand));
                }
            }
            catch (Exception ex)
            {
                return new QueryResult(ex);
            }

            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
            }

            try
            {
                if (useTrasaction)
                {
                    transaction = this.DbConnection.BeginTransaction();
                }

                var command = this.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlCommand;

                if (useTrasaction)
                {
                    command.Transaction = transaction;
                }

                command.ExecuteNonQuery();
                if (useTrasaction)
                {
                    transaction.Commit();
                }

                queryResult = queryResult.CreateSuccessResult();
            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);

                if (useTrasaction && transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollback)
                    {
                        queryResult = new QueryResult(rollback);
                    }
                }
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return queryResult;
        }

        /// <summary>
        /// Single Execute
        /// </summary>
        /// <param name="sqlCommand"></param>
        /// <returns>DataTable in QueryResult</returns>
        public QueryResult ExecuteRawSQLQuery(string sqlCommand)
        {
            QueryResult queryResult = null;
            DataTable dataTable = null;
            IDataReader dbReader = null;

            //=>Check parameter
            try
            {
                if (string.IsNullOrEmpty(sqlCommand))
                {
                    throw new ArgumentNullException(nameof(sqlCommand));
                }
            }
            catch (Exception ex)
            {
                return new QueryResult(ex);
            }

            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
            }

            try
            {

                var command = this.CreateCommand();
                command.CommandType = System.Data.CommandType.Text;
                command.CommandText = sqlCommand;
                dbReader = command.ExecuteReader();

                dataTable = new DataTable();
                dataTable.Load(dbReader);
                queryResult = new QueryResult(dataTable);
            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);

            }
            finally
            {
                if (dataTable != null)
                {
                    dataTable.Dispose();
                }

                if (dbReader != null)
                {
                    dbReader.Close();
                    dbReader.Dispose();
                }

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return queryResult;
        }

        /// <summary>
        /// Custom for send sql command to execute
        /// </summary>
        /// <param name="dbConnectionDelegate"></param>
        /// <returns></returns>
        public QueryResult ExecuteSQLCommand(Func<IDbConnection, IDbTransaction, QueryResult> dbConnectionDelegate, bool useTrasaction = false)
        {
            QueryResult queryResult = null;
            IDbTransaction transaction = null;

            if (dbConnectionDelegate == null)
            {
                throw new ArgumentNullException(nameof(dbConnectionDelegate));
            }
            try
            {
                if (this.DbConnection.State == ConnectionState.Closed)
                {
                    this.DbConnection.Open();
                }
            }
            catch
            {
                this.ReInstance();
                this.DbConnection.Open();
            }

            try
            {
                if (useTrasaction)
                {
                    transaction = this.DbConnection.BeginTransaction();
                }

                queryResult = dbConnectionDelegate.Invoke(this.DbConnection, transaction);

                if (useTrasaction)
                {
                    transaction.Commit();
                }

            }
            catch (Exception ex)
            {
                queryResult = new QueryResult(ex);

                if (useTrasaction && transaction != null)
                {
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception rollback)
                    {
                        queryResult = new QueryResult(rollback);
                    }
                }
            }
            finally
            {
                if (transaction != null)
                {
                    transaction.Dispose();
                }

                if (this.DbConnection.State == ConnectionState.Open)
                {
                    this.DbConnection.Close();
                }
            }

            return queryResult;
        }
    }
}
