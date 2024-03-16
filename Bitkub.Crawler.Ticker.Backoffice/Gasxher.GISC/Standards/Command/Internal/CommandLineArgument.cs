using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Command.Internal
{
    public interface ICommandLineArgument
    {
        string[] GetArguments();

        void SetArguments(string[] args);
    }
}
