using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Storage.Internal
{
    public class FileDetail
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

        public string FullName { get; set; }

        public string Ext { get; set; }

        public string MimeType { get; set; }

        public long ContentLength { get; set; }

        //=>Auto Stream Enable
        private byte[] RawFile { get; set; }

        public byte[] GetRawFile()
        {
            if (this.RawFile != null && this.RawFile.Length > 0) { return this.RawFile; }

            return System.IO.File.ReadAllBytes(this.FullName);
        }
    }
}
