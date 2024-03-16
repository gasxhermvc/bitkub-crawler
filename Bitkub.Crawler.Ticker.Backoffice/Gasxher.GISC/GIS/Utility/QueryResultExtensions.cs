using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Utility
{
    public static class QueryResultExtensions
    {
        public static QueryResult CreateErrorResult(this QueryResult queryResult, string message)
        {
            var result = new QueryResult();
            result.Success = false;
            result.Message = message;
            result.DataTable = new DataTable();
            result.Total = 0;

            return result;
        }


        public static QueryResult CreateSuccessResult(this QueryResult queryResult, string message = "")
        {
            var result = new QueryResult();
            result.Success = true;
            result.Message = message;
            result.DataTable = new DataTable();
            result.Total = 0;

            return result;
        }

        public static QueryResult CreateSuccessResult(this QueryResult queryResult, Dictionary<string, object> data)
        {
            var result = new QueryResult();
            var dataTable = data.ToDataTable();
            result.Success = true;
            result.Message = string.Empty;
            result.DataTable = dataTable;
            result.Total = dataTable.Rows.Count;

            return result;
        }

        internal static DataTable ToDataTable(this Dictionary<string, object> data)
        {
            DataTable dataTable = new DataTable();

            //=>Create Column
            foreach (var key in data.Keys)
            {
                dataTable.Columns.Add(new DataColumn(key, data[key].GetType()));
            }

            //=>Create Row
            var row = dataTable.NewRow();
            foreach (var key in data.Keys)
            {
                row[key] = data[key];
                dataTable.Rows.Add(row);
            }

            return dataTable;
        }
    }
}
