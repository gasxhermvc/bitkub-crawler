using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.DataStore
{
    public interface IDataStore
    {
        /// <summary>
        /// Set multiple data
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="dataStore"></param>
        public void SetDataStore(string storeName, Dictionary<string, object> dataStore);


        /// <summary>
        /// Set single data
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public void SetDataStore(string storeName, string keyName, object value);



        /// <summary>
        /// Get single data with object type
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        object GetValue(string storeName, string keyName);


        /// <summary>
        /// Get single data with generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public T GetValue<T>(string storeName, string keyName);


        /// <summary>
        /// Get dataStore in storage
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDataStore(string storeName);
    }
}
