using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Utility
{
    public class QueryParameter
    {
        public bool IsUseSession = false;
        public Dictionary<string, object> Parameter;

        public QueryParameter()
        {
            this.Parameter = new Dictionary<string, object>();
        }

        public QueryParameter(Dictionary<string, object> param)
        {
            try
            {
                this.Parameter = param;
            }
            catch (Exception ex) { throw ex; }
        }

        public object this[string key]
        {
            get
            {
                if (this.Parameter != null && this.Parameter.ContainsKey(key))
                {
                    return this.Parameter[key];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                this.Add(key, value);
            }
        }

        public void Add(string paramName, object paramValue)
        {
            try
            {
                if (this.Parameter.ContainsKey(paramName))
                {
                    this.Parameter[paramName] = paramValue;
                }
                else
                {
                    this.Parameter.Add(paramName, paramValue);
                }
            }
            catch { }
        }

        public void Remove(string paramName)
        {
            try
            {
                if (this.Parameter == null) return;
                if (!this.Parameter.ContainsKey(paramName)) return;
                this.Parameter.Remove(paramName);
            }
            catch { }
        }

        public string ToJson()
        {
            try
            {
                return JsonConvert.SerializeObject(this.Parameter);
            }
            catch (Exception ex) { throw ex; }
        }
    }
}
