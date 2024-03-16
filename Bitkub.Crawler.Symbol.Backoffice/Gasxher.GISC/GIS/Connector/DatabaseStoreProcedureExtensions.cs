using GasxherGIS.GIS.Connector.Internal;
using GasxherGIS.GIS.Utility;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public static class DatabaseStoreProcedureExtensions
    {
        public static QueryResult ExecuteStoredProcedure(this IDatabaseConnector dbConnection, string storeName, QueryParameter queryParam)
        {
            var dbConnector = dbConnection as DatabaseConnector;
            var _dbConnection = dbConnection.DbConnection;
            var _provider = dbConnection.Provider;


            StringBuilder logString = null;
            QueryResult qResult = null;

            IDbCommand dbCommand = null;
            IDataReader dbReader = null;
            IDbDataParameter dbParam = null;

            //IDbTransaction dbTrans = null;
            DataTable dt = null;
            string paramName = string.Empty;
            int cursorNum = 1;
            try
            {
                logString = new StringBuilder();
                logString.AppendLine(string.Format("\tSP: {0}", storeName));

                if (_provider.Equals(ProviderFactory.MySQL))
                {
                    throw new Exception("Provider ไม่รองรับการ Execute โดยใช้ StoredProcedure");
                }

                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }

                qResult = new QueryResult();
                dbCommand = dbConnection.CreateCommand(storeName, queryParam, ref logString);

                //dbTrans = _dbConnection.BeginTransaction();
                switch (_provider)
                {
                    case ProviderFactory.MSSQL:
                        dbReader = dbCommand.ExecuteReader();
                        while (!dbReader.IsClosed)
                        {
                            dt = new DataTable();
                            dt.Load(dbReader);
                            if (cursorNum == 1)
                            {
                                qResult.DataTable = dt;
                            }
                            else
                            {
                                qResult.AddOutputParam("data" + cursorNum, UtilExtensions.DataTableToDictionary(dt, dbConnection.DateTimeFormat, dbConnection.CultureInfo));
                            }
                            cursorNum++;
                        }
                        break;
                    default:
                        dbCommand.ExecuteNonQuery();
                        break;
                }
                for (int i = 0; i < dbCommand.Parameters.Count; i++)
                {
                    dbParam = dbCommand.Parameters[i] as IDbDataParameter;
                    if (!dbParam.Direction.Equals(ParameterDirection.Output)) continue;
                    paramName = dbParam.ParameterName;
                    if (dbConnector.ParameterNamePrefix.Length > 0)
                    {
                        paramName = paramName.Replace(dbConnector.ParameterNamePrefix, string.Empty);
                    }

                    if (!dbConnector.ReservedOutputName.Contains(paramName.ToUpper()))
                    {
                        paramName = paramName.Replace(dbConnector.OutputReplace, string.Empty);
                        if (dbParam.Value == null || System.DBNull.Value.Equals(dbParam.Value))
                        {
                            qResult.AddOutputParam(paramName, dbParam.Value);
                        }
                        else if (dbParam.Value is DateTime)
                        {
                            qResult.AddOutputParam(paramName, UtilExtensions.DateTimeToString((dbParam.Value as DateTime?)));
                        }
                        else if (dbParam.Value is IDataReader)
                        {
                            dt = new DataTable();
                            if (dbParam.Value != System.DBNull.Value)
                            {
                                dt.Load(dbParam.Value as IDataReader);

                            }
                            qResult.AddOutputParam(paramName.ToLower(), UtilExtensions.DataTableToDictionary(dt, dbConnector.DateTimeFormat, dbConnector.CultureInfo));
                        }
                        else
                        {
                            if (paramName.IndexOf("NT_") == 0)
                            {
                                qResult.AddNTParam(paramName, dbParam.Value);
                            }
                            else
                            {
                                qResult.AddOutputParam(paramName, dbParam.Value);
                            }
                        }
                    }
                    else if (paramName.Equals("PO_DATA"))
                    {
                        dt = new DataTable();
                        if (dbParam.Value != System.DBNull.Value)
                        {
                            dt.Load(dbParam.Value as IDataReader);
                        }
                        qResult.DataTable = dt;
                    }
                    else if (paramName.Equals("PO_TOTAL"))
                    {
                        if (!string.IsNullOrEmpty(dbParam.Value.ToString()))
                        {
                            qResult.Total = int.Parse(dbParam.Value.ToString());
                        }
                    }
                    else if (paramName.Equals("PO_STATUS"))
                    {
                        if (dbParam.Value.ToString().Equals("1"))
                        {
                            qResult.Success = true;
                        }
                        else
                        {
                            qResult.Success = false;
                        }
                    }
                    else if (paramName.Equals("PO_STATUS_MSG"))
                    {
                        qResult.Message = dbParam.Value.ToString();
                    }
                }

                logString.AppendLine(string.Format("<LOGFUNCTION>"));
                if (queryParam.Parameter.ContainsKey("FN_ID") == true)
                {
                    logString.AppendLine(string.Format("{0}", queryParam.Parameter["FN_ID"]));
                }
                logString.AppendLine(string.Format("</LOGFUNCTION>"));


                if (qResult.Success != true)
                {
                    logString.AppendLine(string.Format("{0}", qResult.Message));

                    logString.AppendLine("!!! Error !!!");
                    dbConnector.Logger.LogInformation(logString.ToString());
                }
                else
                {
                    logString.AppendLine("!!! Completed !!!");
                    dbConnector.Logger.LogInformation(logString.ToString());
                }
            }
            catch (Exception ex)
            {
                //try
                //{
                //    dbTrans.Rollback();
                //}
                //catch { }
                qResult = new QueryResult(ex);
                logString.AppendLine(string.Format("{0}", qResult.Message));

                logString.AppendLine(string.Format("<LOGFUNCTION>"));
                if (queryParam.Parameter.ContainsKey("FN_ID") == true)
                {
                    logString.AppendLine(string.Format("{0}", queryParam.Parameter["FN_ID"]));
                }
                logString.AppendLine(string.Format("</LOGFUNCTION>"));

                logString.AppendLine("!!! Error !!!");
                dbConnector.Logger.LogError(logString.ToString());
            }
            finally
            {
                //try
                //{
                //    dbTrans.Dispose();
                //    dbTrans = null;
                //}
                //catch { }
                if (_dbConnection != null && _dbConnection.State == ConnectionState.Open)
                {
                    _dbConnection.Close();
                }

                logString.Clear();
            }
            return qResult;
        }

        public static IDbCommand CreateCommand(this IDatabaseConnector dbConnection)
        {
            var dbConnector = dbConnection as DatabaseConnector;
            var _dbConnection = dbConnection.DbConnection;
            var _provider = dbConnection.Provider;

            IDbCommand _dbCommand = null;
            _dbCommand = _dbConnection.CreateCommand();
            _dbCommand.CommandType = CommandType.Text;
            _dbCommand.CommandTimeout = dbConnector.CommandTimeout;
            if (_provider.Equals(ProviderFactory.Oracle))
            {
                var oracleCommandBindByName = dbConnector.CommandType.GetProperty("BindByName");
                oracleCommandBindByName.SetValue(_dbCommand, true, null);
            }

            return _dbCommand;
        }

        public static IDbCommand CreateCommand(this IDatabaseConnector dbConnection, string StrSQL)
        {
            var dbConnector = dbConnection as DatabaseConnector;
            var _dbConnection = dbConnection.DbConnection;
            var _provider = dbConnection.Provider;

            string StrSQL2 = StrSQL;
            IDbCommand _dbCommand = null;
            _dbCommand = _dbConnection.CreateCommand();
            _dbCommand.CommandType = CommandType.Text;
            _dbCommand.CommandTimeout = dbConnector.CommandTimeout;
            if (_provider.Equals(ProviderFactory.Oracle))
            {
                var oracleCommandBindByName = dbConnector.CommandType.GetProperty("BindByName");
                oracleCommandBindByName.SetValue(_dbCommand, true, null);
            }

            string[] paramName = new string[] { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
            for (int i = StrSQL2.IndexOf("?"); i != -1; i = StrSQL2.IndexOf("?", i))
            {
                var regex = new System.Text.RegularExpressions.Regex(System.Text.RegularExpressions.Regex.Escape("?"));
                StrSQL = regex.Replace(StrSQL, ":PI_" + paramName[_dbCommand.Parameters.Count], 1);
                object _dbParam = Activator.CreateInstance(dbConnector.ParameterType, new object[] { "PI_" + paramName[_dbCommand.Parameters.Count], "" });
                _dbCommand.Parameters.Add(_dbParam);
                i++;
            }
            _dbCommand.CommandText = StrSQL;
            return _dbCommand;
        }

        public static IDbCommand CreateCommand(this IDatabaseConnector dbConnection, string storeName, QueryParameter queryParam, ref StringBuilder logString)
        {
            IDbDataParameter dbParam = null;
            //List<IDbDataParameter> paramList = null;
            IDbCommand dbCommand = null;
            string[] encParameter = null;

            //=>Pointer
            DatabaseConnector dbConnector = dbConnection as DatabaseConnector;

            var _inputReplace = dbConnector.InputReplace;
            var _dbConnection = dbConnector.DbConnection;
            var _provider = dbConnection.Provider;

            try
            {
                if (_dbConnection.State != ConnectionState.Open)
                {
                    _dbConnection.Open();
                }

                dbCommand = _dbConnection.CreateCommand();
                dbCommand.CommandType = CommandType.StoredProcedure;
                dbCommand.CommandText = storeName;
                dbCommand.CommandTimeout = dbConnector.CommandTimeout;

                if (_provider.Equals(ProviderFactory.Oracle))
                {
                    var oracleCommandBindByName = dbConnector.CommandType.GetProperty("BindByName");
                    oracleCommandBindByName.SetValue(dbCommand, true, null);
                }

                //if (System.Configuration.ConfigurationManager.AppSettings["IS_DEBUG"] != "1" && !string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["PARAMETER_ENCRYPT"]))
                //{
                //    encParameter = System.Configuration.ConfigurationManager.AppSettings["PARAMETER_ENCRYPT"].Split('|');
                //}
                dbConnector.CommandBuilderType.GetMethod("DeriveParameters").Invoke(null, new object[] { dbCommand });
                for (int i = 0; i < dbCommand.Parameters.Count; i++)
                {
                    string paramName = string.Empty;
                    object paramValue = null;
                    dbParam = dbCommand.Parameters[i] as IDbDataParameter;
                    switch (dbParam.Direction)
                    {
                        case ParameterDirection.Input:
                            if (queryParam != null && queryParam.Parameter != null)
                            {
                                if (dbParam.ParameterName.IndexOf(_inputReplace) != -1)
                                {
                                    paramName = dbParam.ParameterName.Substring(dbParam.ParameterName.IndexOf(_inputReplace) + _inputReplace.Length);
                                }
                                else
                                {
                                    paramName = dbParam.ParameterName;
                                }

                                if (queryParam.Parameter.ContainsKey(paramName.ToUpper()))
                                {
                                    paramValue = queryParam.Parameter[paramName];
                                }

                                if (paramValue != null && UtilExtensions.DBTypeMap(dbParam.DbType).Equals(typeof(DateTime)) || dbParam.Value is DateTime)
                                {
                                    if (paramValue is string)
                                    {
                                        dbParam.Value = UtilExtensions.StringToDateTime(paramValue.ToString());
                                    }
                                    else if (paramValue is int)
                                    {
                                        dbParam.Value = UtilExtensions.UnixTimeStampToDateTime(int.Parse(paramValue.ToString())).AddMinutes(dbConnector.TimeZoneOffset);
                                    }
                                    else if (paramValue is long)
                                    {
                                        dbParam.Value = UtilExtensions.UnixTimeStampToDateTime(long.Parse(paramValue.ToString())).AddMinutes(dbConnector.TimeZoneOffset);
                                    }

                                    if (_provider == ProviderFactory.MSSQL)
                                    {
                                        logString.AppendLine(string.Format("{0}: PARSE('{1}' as DATETIME USING 'th-TH')", dbParam.ParameterName, UtilExtensions.DateTimeToString(dbParam.Value as DateTime?)));
                                    }
                                    else if (_provider == ProviderFactory.Oracle)
                                    {
                                        logString.AppendLine(string.Format("{0}: TO_DATE('{1}','DD/MM/YYYY HH24:MI:SS','NLS_DATE_LANGUAGE=THAI')", dbParam.ParameterName, UtilExtensions.DateTimeToString(dbParam.Value as DateTime?)));
                                    }

                                    break;
                                }
                                else if (paramValue != null)
                                {
                                    if (dbParam.DbType != DbType.String && !string.IsNullOrEmpty(paramValue.ToString()))
                                    {
                                        dbParam.Value = paramValue;
                                    }
                                    else if (dbParam.DbType == DbType.String)
                                    {
                                        dbParam.Value = paramValue;
                                    }
                                    else
                                    {
                                    }
                                }
                                else
                                {
                                    dbParam.Value = System.DBNull.Value;
                                }
                            }
                            object logValue = dbParam.Value;
                            if (encParameter != null && encParameter.Contains(dbParam.ParameterName.Replace(_inputReplace, "").ToUpper()))
                            {

                                logValue = "-- Secured Field --";
                            }
                            logString.AppendLine(string.Format("{0}: {1}", dbParam.ParameterName, logValue));
                            break;
                        case ParameterDirection.InputOutput:
                        case ParameterDirection.Output:
                            dbParam.Direction = ParameterDirection.Output;
                            break;
                        //paramName = dbParam.ParameterName.Substring(dbParam.ParameterName.IndexOf(_outputReplace) + _outputReplace.Length);
                        //break;
                        default:
                            continue;
                    }
                    //paramList.Add(dbParam);
                }
                return dbCommand;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void CreatePO(this IDbCommand command)
        {
            SqlParameter status = new SqlParameter();
            status.ParameterName = "@PO_STATUS";
            status.SqlDbType = SqlDbType.Int;
            status.Value = (object)1;
            command.Parameters.Add(status);

            SqlParameter msg = new SqlParameter();
            msg.ParameterName = "@PO_STATUS_MSG";
            msg.SqlDbType = SqlDbType.VarChar;
            msg.Value = (object)"";
            command.Parameters.Add(msg);

            SqlParameter total = new SqlParameter();
            total.ParameterName = "@PO_TOTAL";
            total.SqlDbType = SqlDbType.Int;
            total.Value = (object)0;
            command.Parameters.Add(total);
        }
    }
}
