using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.Utility
{
    public class QueryResult
    {
        private bool _success = true;
        private int _total = -1;
        private string _message = string.Empty;
        private DataTable _dataTable = null;
        private Dictionary<string, object> _outputParameters = null;
        private Dictionary<string, object> _ntParameters = null;
        private Dictionary<string, object> _serializeObject = null;

        public bool Success
        {
            get => _success;
            set => _success = value;
        }

        public int Total
        {
            get => _total;
            set => _total = value;
        }

        public string Message
        {
            get => _message;
            set => _message = value;
        }

        public DataTable DataTable
        {
            get => _dataTable;
            set => _dataTable = value;
        }

        public Dictionary<string, object> OutputParameters => _outputParameters;

        public Dictionary<string, object> NTParameters => _ntParameters;

        public QueryResult()
        {
            this._dataTable = new DataTable();
            this._outputParameters = new Dictionary<string, object>();
        }

        public QueryResult(DataTable data)
        {
            this._dataTable = data;
            this._outputParameters = new Dictionary<string, object>();
        }

        public QueryResult(Exception ex)
        {
            Exception exception = ex.GetBaseException();
            exception = exception == null ? ex : exception;
            this._dataTable = new DataTable();
            this._outputParameters = new Dictionary<string, object>();
            try
            {
                this._outputParameters.Add("error", new Dictionary<string, object>()
                {
                    {"source",exception.Source},
                    {"stackTrace",exception.StackTrace},
                    {"hresult",exception.HResult}
                });
            }
            catch { }
            this._message = exception.ToString();
            this._success = false;
            this._total = -1;
        }

        public void AddOutputParam(string paramName, object value)
        {
            try
            {
                if (this._outputParameters == null)
                    this._outputParameters = new Dictionary<string, object>();
                this._outputParameters.Add(paramName, value);
            }
            catch { }
        }

        public void RemoveOutputParam(string paramName)
        {
            try
            {
                if (this._outputParameters == null) return;
                this._outputParameters.Remove(paramName);
            }
            catch { }
        }

        public void AddNTParam(string paramName, object value)
        {
            try
            {
                if (this._ntParameters == null)
                    this._ntParameters = new Dictionary<string, object>();
                this._ntParameters.Add(paramName, value);
            }
            catch { }
        }

        public void RemoveNTParam(string paramName)
        {
            try
            {
                if (this._ntParameters == null) return;
                this._ntParameters.Remove(paramName);
            }
            catch { }
        }

        public string ToJson()
        {
            try
            {
                this.ToDictionary();
                return JsonConvert.SerializeObject(this._serializeObject);
            }
            catch (Exception ex) { throw ex; }

        }

        public Dictionary<string, object> ToDictionary()
        {
            try
            {
                this._serializeObject = new Dictionary<string, object>();
                this._serializeObject.Add("success", this.Success);
                if (this.Total == 0 || this.Total == -1 && this._dataTable != null)
                {
                    this._serializeObject.Add("total", this._dataTable.Rows.Count);
                }
                else
                {
                    this._serializeObject.Add("total", this.Total);
                }
                this._serializeObject.Add("message", this.Message);
                this._serializeObject.Add("data", UtilExtensions.DataTableToDictionary(this._dataTable));
                foreach (var param in OutputParameters)
                {
                    if (this._serializeObject.ContainsKey(param.Key))
                    {
                        this._serializeObject[param.Key] = param.Value;
                    }
                    else
                    {
                        this._serializeObject.Add(param.Key, param.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                this._serializeObject = new Dictionary<string, object>();
                this._serializeObject.Add("success", false);
                this._serializeObject.Add("total", -1);
                this._serializeObject.Add("message", "ไม่สามารถสร้าง Dictionary ได้เนื่องจาก : " + ex.Message);
                this._serializeObject.Add("data", null);
                this._serializeObject.Add("error", new Dictionary<string, object>()
                {
                    {"source",ex.Source},
                    {"stackTrace",ex.StackTrace},
                    {"hresult",ex.HResult}
                });
            }
            return this._serializeObject;
        }
    }
}
