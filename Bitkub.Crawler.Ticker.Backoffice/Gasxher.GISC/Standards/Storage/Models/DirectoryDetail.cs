using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Storage.Internal
{
    public class DirectoryDetail
    {
        public string DirectoryName { get; set; }
        public string FilePath { get; set; }
        public List<FileDetail> Files { get; set; }
        public long Count => Files?.Count ?? 0;
        public NetworkPathDetail networkPathAccess { get; set; }
    }
}
