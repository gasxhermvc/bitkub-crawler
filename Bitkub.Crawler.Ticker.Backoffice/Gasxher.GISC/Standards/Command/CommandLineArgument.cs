using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GasxherGIS.Standards.Command.Internal;

namespace GasxherGIS.Standards.Command
{
    public class CommandLineArgument : ICommandLineArgument
    {
        private string[] arguments { get; set; } = new string[] { };

        public string[] GetArguments()
        {
            return arguments ?? new string[] { };
        }

        public void SetArguments(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            this.arguments = args;
        }
    }
}
