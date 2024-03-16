using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Standards.Logging.Extensions
{
    public static class LoggerHandlerExtensions
    {
        public static string RemoveLogNamespace(this ILogProvider logProvider, string loggerName)
        {
            return !logProvider.IsLogNamespace ?
                loggerName.Split('.')?.LastOrDefault() ?? loggerName : loggerName;
        }

        public static string LogLevelColor(this LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Information:
                    return $"\x1B[32m{logLevel}\x1B[39m\x1B[22m";
                case LogLevel.Debug:
                    return $"\x1B[35m{logLevel}\x1B[39m\x1B[22m";
                case LogLevel.Trace:
                    return $"\x1B[36m{logLevel}\x1B[39m\x1B[22m";
                case LogLevel.Error:
                    return $"\x1B[1m\x1B[31m{logLevel}\x1B[39m\x1B[22m";
                case LogLevel.Warning:
                    return $"\x1B[1m\x1B[33m{logLevel}\x1B[39m\x1B[22m";
                case LogLevel.Critical:
                    return $"\x1B[33m{logLevel}\x1B[39m\x1B[22m";
                default:
                    return $"\x1B[39m\x1B[22m{logLevel}\x1B[39m\x1B[22m";
            }
        }

        public static string LogLevelElipsis(this LogLevel logLevel, string tempplate)
        {
            return tempplate.Replace(logLevel.ToString(), MapLevel(logLevel).ToUpper());

            string MapLevel(LogLevel logLevel)
            {
                return logLevel switch
                {
                    LogLevel.Information => "Info",
                    LogLevel.Warning => "Warn",
                    _ => logLevel.ToString()
                };
            }
        }
    }
}
