using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Storage.Internal
{
    public class NetworkPathDetail
    {
        public string RemotePath { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Domain { get; set; }

        public ConnectionState connectionState { get; set; } = ConnectionState.Disconnected;
    }

    public enum ConnectionState
    {
        Disconnected = 0,
        Connected = 1,
        CannotAccessNetwork = 9
    }
}
