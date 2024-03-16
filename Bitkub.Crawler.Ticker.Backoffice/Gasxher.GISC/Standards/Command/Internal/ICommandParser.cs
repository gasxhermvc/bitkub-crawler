using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Command.Internal
{
    public interface ICommandParser
    {
        void AddArguments(params string[] args);

        void AddArgumentWithDefault(string key, object value, bool overrideValue = true);

        void ClearArguments();

        T GetValue<T>(string argumentName);
    }
}
