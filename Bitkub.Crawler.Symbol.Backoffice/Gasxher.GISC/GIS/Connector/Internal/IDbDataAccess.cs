using GasxherGIS.GIS.Utility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Connector.Internal
{
    public interface IDbDataAccess
    {
        DbConfigProvider Options { get; }

        /// <summary>
        /// ใช้ DataSource ของ DbConfigProvider ที่มีค่าเท่ากับ Default <see cref="DbConfigProvider"/
        /// </summary>
        /// <returns></returns>
        IDatabaseConnector DbConnection();


        /// <summary>
        /// ใช้ DataSource ของ DbConfigProvider ที่มีค่าเท่ากับ dataSourceName <see cref="DbConfigProvider"/
        /// </summary>
        /// <param name="dataSourceName"></param>
        /// <returns></returns>
        IDatabaseConnector DbConnection(string dataSourceName);
    }
}
