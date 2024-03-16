using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon
{
    public class HttpParameter
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public HttpParameter() { }

        public HttpParameter(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
