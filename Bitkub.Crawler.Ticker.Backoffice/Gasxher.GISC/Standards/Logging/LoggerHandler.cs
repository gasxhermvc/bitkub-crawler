using System;
using Microsoft.Extensions.Logging;
using GasxherGIS.Standards.Logging.Internal;
using GasxherGIS.Standards.Logging.Manager;

namespace GasxherGIS.Standards.Logging
{
    public class LoggerHandler : ILogger
    {
        private readonly string _name;
        private readonly ILogProvider _appLoggerProvider;
        private readonly LogHandler _handler;

        public LoggerHandler(string name, ILogProvider loggerProvider)
        {
            if (loggerProvider.provider == null)
            {
                //=>ตรวจสอบ Configuration มี Provider Application หรือไม่
                throw new ArgumentNullException(nameof(loggerProvider.provider));
            }
            _name = name;
            _appLoggerProvider = loggerProvider;
            _handler = loggerProvider.provider.GetHandler();
        }

        public IDisposable BeginScope<TState>(TState state) => default;

        public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string messageTemplate = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.ffff")} %level% {_name}[{eventId}] - {formatter(state, exception)}";

            if (_appLoggerProvider.IsConsole)
            {
                Console.WriteLine(messageTemplate.Replace("%level%", this.LogLevelColor(logLevel)));
            }

            _handler.Append(messageTemplate.Replace("%level%", logLevel.ToString()));
        }

        private string LogLevelColor(LogLevel logLevel)
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
    }
}
