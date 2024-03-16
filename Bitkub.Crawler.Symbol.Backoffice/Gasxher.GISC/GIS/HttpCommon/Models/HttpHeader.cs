using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.GIS.HttpCommon
{
    public class HttpHeader
    {
        public string Name { get; set; }

        public string Value { get; set; }

        public HttpHeader() { }

        public HttpHeader(string name, string value)
        {
            this.Name = name;
            this.Value = value;
        }
    }
}
