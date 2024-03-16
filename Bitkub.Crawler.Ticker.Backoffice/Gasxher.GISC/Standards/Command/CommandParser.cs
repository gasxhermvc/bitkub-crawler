using System;
using System.Collections.Generic;
using System.Linq;
using GasxherGIS.Standards.Command.Internal;

namespace GasxherGIS.Standards.Command
{
    public class CommandParser : ICommandParser
    {
        private Dictionary<string, object> arguments = new Dictionary<string, object>();

        public CommandParser(ICommandLineArgument commandLineArgument)
        {
            this.AddArguments(commandLineArgument.GetArguments());
        }

        public void AddArguments(params string[] args)
        {
            //=>Parser สำหรับเริ่มต้นเมื่อ Class CommandParser ถูกสร้างขึ้น
            var arguments = this.Parser(args);
            this.MergeArguments(arguments);
        }

        public void ClearArguments()
        {
            this.arguments = new Dictionary<string, object>();
        }

        public void AddArgumentWithDefault(string key, object value, bool overrideValue = true)
        {
            if (this.arguments.ContainsKey(key))
            {
                if (overrideValue)
                {
                    this.arguments[key] = value;
                }
            }
            else
            {
                this.arguments.Add(key, value);
            }
        }

        public T GetValue<T>(string argumentName)
        {
            object value;

            try
            {
                if (this.arguments.TryGetValue(argumentName, out value))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }

            //try
            //{
            //    if (value.GetType() == typeof(bool))
            //    {
            //        return (T)Convert.ChangeType(value, typeof(T));
            //    }

            //    if (value == null)
            //    {
            //        return default(T);
            //    }

            //    return (T)value;

            //}
            //catch
            //{
            //    return default(T);
            //}
        }

        private Dictionary<string, object> Parser(string[] args)
        {
            Dictionary<string, object> arguments = new Dictionary<string, object>();

            var argumentsSplits = string.Join("|", args)
                .Split('|')
                .Where(w => !string.IsNullOrEmpty(w))
                .ToList();

            if (argumentsSplits == null || argumentsSplits.Count < 1)
            {
                return arguments;
            }

            argumentsSplits.ForEach(argument =>
            {
                var padding = 2;
                var index = 1;
                string key = string.Empty;

                var argumentSplit = argument.Split('=');

                foreach (var arg in argumentSplit)
                {
                    if (index == padding)
                    {
                        arguments.Add(key, (object)arg);
                        key = string.Empty;
                        index = 1;
                        continue;
                    }

                    key = arg.Replace("--", string.Empty).Replace("=", string.Empty).Replace(" ", string.Empty).Trim();

                    index++;
                }
            });




            return arguments;
        }

        private void MergeArguments(Dictionary<string, object> newArguments)
        {
            foreach (var key in newArguments.Keys)
            {
                if (!this.arguments.ContainsKey(key))
                {
                    //=>Add
                    this.arguments.Add(key, newArguments[key]);
                }
                else
                {
                    //=>Update
                    this.arguments[key] = newArguments[key];
                }
            }
        }
    }
}
