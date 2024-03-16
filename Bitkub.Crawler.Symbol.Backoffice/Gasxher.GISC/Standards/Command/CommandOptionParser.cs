using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using GasxherGIS.Standards.Command.Internal;

namespace GasxherGIS.Standards.Command
{
    public class CommandOptionParser : ICommandOptionParser
    {
        private Dictionary<string, object> options = new Dictionary<string, object>();

        private readonly string[] _args;

        public CommandOptionParser(ICommandLineArgument commandLineArgument)
        {
            if (commandLineArgument == null)
            {
                throw new ArgumentNullException(nameof(commandLineArgument));
            }

            _args = commandLineArgument.GetArguments();

            this.AddParser<CommandLineOption>((ParserSettings Settings) =>
            {
                Settings.IgnoreUnknownArguments = true;
                Settings.CaseSensitive = false;
                Settings.CaseInsensitiveEnumValues = false;
            });
        }

        public void AddParser<TSource>(Action<ParserSettings> configureDelegate)
        {
            if (configureDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureDelegate));
            }

            configureDelegate.Invoke(new ParserSettings());

            var parser = new CommandLine.Parser(configuration: configureDelegate);
            parser.ParseArguments<TSource>(_args)
             .WithParsed((TSource opt) =>
             {
                 var options = ToDictionary(opt);
                 this.MergeOption(options);
             });
        }

        public void ClearOption()
        {
            this.options.Clear();
        }

        public TResult GetValue<TResult>(string name)
        {
            object value;
            return this.options.TryGetValue(name, out value) ? (TResult)Convert.ChangeType(value, typeof(TResult)) : (TResult)default;
        }

        private Dictionary<string, object> ToDictionary<TSource>(TSource option)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();

            var props = option.GetType().GetProperties();

            foreach (var prop in props)
            {
                var name = prop.Name;

                var value = prop.GetValue(option);

                result.Add(name, value);
            }

            return result;
        }

        private void MergeOption(Dictionary<string, object> newOptions)
        {
            foreach (var key in newOptions.Keys)
            {
                if (!this.options.ContainsKey(key))
                {
                    //=>Add
                    this.options.Add(key, newOptions[key]);
                }
                else
                {
                    //=>Update
                    this.options[key] = newOptions[key];
                }
            }
        }
    }
}
