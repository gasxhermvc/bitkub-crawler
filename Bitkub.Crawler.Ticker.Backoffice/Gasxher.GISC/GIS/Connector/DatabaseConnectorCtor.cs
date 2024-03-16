using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using GasxherGIS.GIS.Utility;
using GasxherGIS.GIS.Connector.Internal;
using Microsoft.Extensions.Logging;

namespace GasxherGIS.GIS.Connector
{
    public partial class DatabaseConnector : IDatabaseConnector, IDisposable
    {
        protected static string _OracleAssembly = "Oracle.DataAccess";
        protected static string _MySqlAssembly = "MySql.Data";
        protected static string _PostgreSqlAssembly = "Npgsql";

        protected IDbConnection _dbConnection = null;
        protected ILogger _logger;
        protected object _dbCoreConnection = null;

        protected DataSource _dataSoruce { get; set; }
        protected ILogger logger { get; set; }

        protected string _assemblyName;
        protected string _connectionTypeName;
        protected string _commandTypeName;
        protected string _commandBuilderTypeName;
        protected string _parameterTypeName;
        protected string _paramNamePrefix;

        public string ParameterNamePrefix
        {
            get => _paramNamePrefix;
        }

        public DataSource DataSource { get => _dataSoruce; }

        protected static ConcurrentDictionary<string, Assembly> _dictAssemblyCache = new ConcurrentDictionary<string, Assembly>();
        protected Type _connectionType;
        protected Type _commandType;
        protected Type _commandBuilderType;
        protected Type _parameterType;

        public Type ConnectionType { get => _connectionType; }
        public Type CommandType { get => _commandType; }
        public Type CommandBuilderType { get => _commandBuilderType; }
        public Type ParameterType { get => _parameterType; }

        public string InputReplace { get => _inputReplace; }
        public string OutputReplace { get => _outputReplace; }

        protected string _inputReplace = "PI_";
        protected string _outputReplace = "PO_";

        protected Exception _exceptionTemporary = null;

        protected string[] _reservedOutputName = new string[] { "PO_DATA", "PO_STATUS", "PO_STATUS_MSG", "PO_TOTAL" }; // ParameterName not to remove "PO_" when return

        public string[] ReservedOutputName { get => _reservedOutputName; }

        protected string _connectionString = "";

        public string ConnectionString
        {
            get
            {
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }


        protected int _connectionTimeout = 300;

        public int ConnectionTimeout
        {
            get
            {
                return _connectionTimeout;
            }
            set
            {
                _connectionTimeout = value;
            }
        }


        protected int _commandTimeout = 60000;

        public int CommandTimeout
        {
            get
            {
                return _commandTimeout;
            }
            set
            {
                _commandTimeout = value;
            }
        }


        protected ProviderFactory _provider = ProviderFactory.MSSQL;

        public ProviderFactory Provider
        {
            get
            {
                return _provider;
            }
            set
            {
                _provider = value;
            }
        }


        protected string _assemblyVersion = "";

        public string AssemblyVersion
        {
            get
            {
                return _assemblyVersion;
            }
            set
            {
                _assemblyVersion = value;
            }
        }


        protected System.Globalization.CultureInfo _cultureInfo = new System.Globalization.CultureInfo("th-TH");
        /// <summary>
        /// default is System.Globalization.CultureInfo("th-TH")
        /// </summary>
        public System.Globalization.CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set { _cultureInfo = value; }
        }


        protected string _dateTimeFormat = "dd/MM/yyyy HH:mm:ss";
        /// <summary>
        /// default is "dd/MM/yyyy HH:mm:ss"
        /// </summary>
        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { _dateTimeFormat = value; }
        }

        protected double _timeZoneOffset = -420;
        public double TimeZoneOffset
        {
            get { return _timeZoneOffset; }
            set { _timeZoneOffset = value; }
        }

        public IDbConnection DbConnection => _dbConnection;

        public ILogger Logger => _logger;

        public DatabaseConnector(ILogger logger, string connectionString, ProviderFactory provider, DataSource DataSource)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (DataSource == null)
            {
                throw new ArgumentNullException(nameof(DataSource));
            }

            _logger = logger;

            _connectionString = connectionString;
            _provider = provider;
            _dataSoruce = DataSource;

            this.ReInstance();
        }

        public void ReInstance()
        {
            try
            {
                switch (_provider)
                {
                    case ProviderFactory.MSSQL:
                        _paramNamePrefix = "@";
                        _connectionType = typeof(System.Data.SqlClient.SqlConnection);
                        _commandType = typeof(System.Data.SqlClient.SqlCommand);
                        _commandBuilderType = typeof(System.Data.SqlClient.SqlCommandBuilder);
                        _parameterType = typeof(System.Data.SqlClient.SqlParameter);
                        break;
                    case ProviderFactory.Oracle:
                        _paramNamePrefix = string.Empty;
                        _assemblyName = _OracleAssembly;
                        _connectionTypeName = string.Format("{0}.Client.OracleConnection", _assemblyName);
                        _commandTypeName = string.Format("{0}.Client.OracleCommand", _assemblyName);
                        _commandBuilderTypeName = string.Format("{0}.Client.OracleCommandBuilder", _assemblyName);
                        _parameterTypeName = string.Format("{0}.Client.OracleParameter", _assemblyName);
                        break;
                    case ProviderFactory.MySQL:
                        _paramNamePrefix = string.Empty;
                        _assemblyName = _MySqlAssembly;
                        _connectionTypeName = "MySql.Data.MySqlClient.MySqlConnection";
                        _commandTypeName = "MySql.Data.MySqlClient.MySqlCommand";
                        _commandBuilderTypeName = "MySql.Data.MySqlClient.MySqlCommandBuilder";
                        _parameterTypeName = "MySql.Data.MySqlClient.MySqlParameter";
                        break;
                    case ProviderFactory.PostgreSQL:
                        _paramNamePrefix = ":";
                        _assemblyName = _PostgreSqlAssembly;
                        _connectionTypeName = "Npgsql.NpgsqlConnection";
                        _commandTypeName = "Npgsql.NpgsqlCommand";
                        _commandBuilderTypeName = "Npgsql.NpgsqlCommandBuilder";
                        _parameterTypeName = "Npgsql.NpgsqlParameter";
                        break;
                }

                if (_connectionType == null)
                {
                    if (!_dictAssemblyCache.ContainsKey(_assemblyName))
                    {
                        _dictAssemblyCache.TryAdd(_assemblyName, GasxherGIS.GIS.Utility.AssemblyExtensions.AssemblyFromName(_assemblyName));
                    }
                    _connectionType = _dictAssemblyCache[_assemblyName].GetType(_connectionTypeName);
                    _commandType = _dictAssemblyCache[_assemblyName].GetType(_commandTypeName);
                    _commandBuilderType = _dictAssemblyCache[_assemblyName].GetType(_commandBuilderTypeName);
                    _parameterType = _dictAssemblyCache[_assemblyName].GetType(_parameterTypeName);
                }

                _dbCoreConnection = Activator.CreateInstance(_connectionType, new object[] { _connectionString });
                _dbConnection = _dbCoreConnection as IDbConnection;
            }
            catch (Exception ex)
            {
                _exceptionTemporary = ex;
            }
        }

        public void Dispose()
        {
            if (this._dbConnection != null)
            {
                if (this._dbConnection.State == ConnectionState.Open)
                {
                    this._dbConnection.Close();
                }

                this._dbConnection.Dispose();
            }
        }
    }
}
