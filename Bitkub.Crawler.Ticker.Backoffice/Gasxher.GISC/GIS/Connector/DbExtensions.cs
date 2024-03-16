using GasxherGIS.GIS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector
{
    public static class DbExtensions
    {
        public static QueryResult ExecuteReaderResult(this IDbCommand cmd)
        {
            IDataReader dataReader = null;

            dataReader = cmd.ExecuteReader();
            DataTable dataTable = new DataTable();
            dataTable.Load(dataReader);

            return new QueryResult(dataTable);
        }

        public static QueryResult ExecuteNonQueryResult(this IDbCommand cmd)
        {
            QueryResult queryResult = null;
            cmd.ExecuteNonQuery();
            return queryResult.CreateSuccessResult();
        }
    }
}
