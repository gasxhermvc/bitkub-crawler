using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.DataStore
{
    public class DataStore : IDataStore
    {
        /// <summary>
        /// source(key) string = step
        /// value is Dictionary <key,value>
        /// </summary>
        private Dictionary<string, Dictionary<string, object>> storage = new Dictionary<string, Dictionary<string, object>>();

        public DataStore() { }

        public Dictionary<string, object> GetDataStore(string storeName, string keyName)
        {
            if (!storage.ContainsKey(storeName))
            {
                return null;
            }

            return storage[storeName];
        }

        /// <summary>
        /// Get single data with object type
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public object GetValue(string storeName, string keyName)
        {
            if (!storage.ContainsKey(storeName))
            {
                return null;
            }

            object value;
            return storage[storeName].TryGetValue(keyName, out value) ? value : null;
        }

        /// <summary>
        /// Get single data with generic type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public T GetValue<T>(string storeName, string keyName)
        {
            if (!storage.ContainsKey(storeName))
            {
                return default(T);
            }

            object value;
            return storage[storeName].TryGetValue(keyName, out value) ? (T)value : default(T);
        }


        /// <summary>
        /// Get dataStore in storage
        /// </summary>
        /// <param name="storeName"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDataStore(string storeName)
        {
            if (storage.ContainsKey(storeName))
            {
                return storage[storeName];
            }

            return null;
        }


        /// <summary>
        /// Set multiple data
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="dataStore"></param>
        public void SetDataStore(string storeName, Dictionary<string, object> dataStore)
        {
            if (!storage.ContainsKey(storeName))
            {
                //=>Add to storage
                storage.Add(storeName, dataStore);
            }
            else
            {
                //=>Update to storage
                foreach (var key in dataStore.Keys)
                {
                    if (!storage[storeName].ContainsKey(key))
                    {
                        //=>Add to datastore
                        storage[storeName].Add(key, dataStore[key]);
                    }
                    else
                    {
                        //=>Update to datastore
                        storage[storeName][key] = dataStore[key];
                    }
                }
            }
        }

        /// <summary>
        /// Set single data
        /// </summary>
        /// <param name="storeName"></param>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public void SetDataStore(string storeName, string keyName, object value)
        {
            if (!storage.ContainsKey(storeName))
            {
                //=>Add to storage
                storage.Add(storeName, new Dictionary<string, object>() { { keyName, value } });
            }
            else
            {
                //=>Update to storage
                if (!storage[storeName].ContainsKey(keyName))
                {
                    //=>Add to datastore
                    storage[storeName].Add(keyName, value);
                }
                else
                {
                    //=>Update to datastore
                    storage[storeName][keyName] = value;
                }
            }
        }
    }
}
