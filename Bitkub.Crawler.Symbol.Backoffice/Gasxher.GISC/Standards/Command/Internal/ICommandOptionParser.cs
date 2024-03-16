using System;
using CommandLine;

namespace GasxherGIS.Standards.Command.Internal
{
    public interface ICommandOptionParser
    {
        void AddParser<TSource>(Action<ParserSettings> configureDelegate);

        void ClearOption();

        TResult GetValue<TResult>(string name);
    }
}
