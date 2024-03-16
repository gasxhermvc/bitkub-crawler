using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using GasxherGIS.Application;
using Microsoft.Extensions.Options;
using GasxherGIS.Standards.Platform.Internal;
using System.Text.RegularExpressions;
using System.Linq;
using GasxherGIS.Standards.Platform.Extensions;
using GasxherGIS.Extensions;

namespace GasxherGIS.Standards.Logging.Manager
{
    public class LoggerManager
    {

        private const string separatorKey = ":";
        private const string EOF = "%";

        private bool IsFirst = true;
        private readonly IConfiguration _configuration;
        private readonly IPlatformRuntime _platformRuntime;
        private readonly IOptions<LoggingProviders> _loggingProviderOptions;
        private LoggingProviders _provider = null;

        public LoggingProviders LoggingProvider
        {
            get
            {
                if (_provider == null)
                {
                    _provider = (_loggingProviderOptions.Value.Providers != null) ? _loggingProviderOptions.Value :
                        _configuration.GetSection(AppConsoleDefaults.LoggingKey).Get<LoggingProviders>();

                    foreach(var provide in _provider.Providers)
                    {
                        provide.template = provide.Clone<LogTemplateProvider>();
                    }
                }

                return _provider;
            }
        }

        private Dictionary<string, LogProvider> providers = new Dictionary<string, LogProvider>();

        private Dictionary<string, Func<string>> variables => this.createVariable();

        public LoggerManager(IConfiguration configuration,
            IPlatformRuntime platformRuntime,
            IOptions<LoggingProviders> loggingProviderOptions)
        {
            _configuration = configuration;
            _platformRuntime = platformRuntime;
            _loggingProviderOptions = loggingProviderOptions;

        }

        public LogProvider GetLogger(string providerName)
        {
            this.RegisterProvider();

            LogProvider provider;
            return this.providers.TryGetValue(providerName, out provider) ? provider : null;
        }

        private void RegisterProvider()
        {
            if (providers != null && providers.Count > 0)
            {
                return;
            }

            foreach (var provider in LoggingProvider.Providers)
            {
                provider.FilePath = _platformRuntime.FormatPath(provider.FilePath);

                provider.SetHandler(this.CreateLoggerFile(provider));

                providers.Add(provider.Name, provider);
            }
        }

        private LogHandler CreateLoggerFile(LogProvider fileSource)
        {
            fileSource = this.SetupLogProvider(fileSource);

            return new LogHandler(_platformRuntime.CombinePath(fileSource.FilePath, fileSource.FileName));
        }

        private LogProvider SetupLogProvider(LogProvider fileSource)
        {
            var filePath = this.ParseTemplate(_platformRuntime.CombinePath(fileSource.FilePath, fileSource.FileName));
            var ext = System.IO.Path.GetExtension(filePath);

            if (fileSource.Running)
            {
                string[] files = new string[] { };

                if (System.IO.Directory.Exists(_platformRuntime.GetDirectoryName(filePath)))
                {
                    files = System.IO.Directory.GetFiles(_platformRuntime.GetDirectoryName(filePath)
                       , $"*{System.IO.Path.GetExtension(filePath)}");
                }

                var runningNumber = this.GetMaxRunning(
                    System.IO.Path.GetFileName(filePath), files);

                filePath = string.Format("{0}_{1}{2}", filePath.Replace(ext, string.Empty), runningNumber.ToString("00"), ext);
            }

            //=>Update fileSource
            fileSource.FileName = System.IO.Path.GetFileName(filePath);
            fileSource.FilePath = _platformRuntime.GetDirectoryName(filePath);

            return fileSource;
        }

        private int GetMaxRunning(string fileName, string[] Files)
        {
            if (Files.Length == 0) { return 1; }

            var dateOfFile = fileName.Replace(System.IO.Path.GetExtension(fileName), string.Empty);

            var numbers = (from file in Files
                           let fileNameSpliting = System.IO.Path.GetFileName(file)
                                .Replace(System.IO.Path.GetExtension(file), string.Empty).Split('-')
                           let running = fileNameSpliting[^1].Split('_')[^1]
                           let prefix = string.Join("-", fileNameSpliting).Replace($"_{running}", string.Empty)
                           where dateOfFile == prefix
                           select int.Parse(running))
                           .ToList();

            if (numbers.Count == 0) { return 1; }

            var maxNumber = numbers.Max();
            return maxNumber + 1;
        }

        private bool IsDirectory(string filePath)
        {
            if (_platformRuntime.IsWindows())
            {
                //=>Drive
                if (filePath.Contains(":/"))
                {
                    return true;
                }

                //=>Virtual Path
                //=>\\192....
                //=>\\domain...
                if (Regex.IsMatch(filePath, @"^\\(\d+)|^\\\\(\d+)|^\\(w+)"))
                {
                    return true;
                }
            }

            if (_platformRuntime.IsLinux() || _platformRuntime.IsOSX())
            {
                if (filePath.StartsWith("/"))
                {
                    return true;
                }
            }

            return false;
        }

        private string ParseTemplate(string template)
        {
            foreach (var key in this.variables.Keys)
            {
                //=>Set concurrent key
                if (key.Contains(separatorKey))
                {
                    //%format:.*%
                    string exactVariable = Regex.Match(template, string.Format("{0}{1}.*{2}",
                        EOF,
                        this.RemoveEOF(key),
                        EOF)).Value;
                    concurrentVariableKey = !string.IsNullOrEmpty(exactVariable) ? exactVariable : key;
                }
                else
                {
                    concurrentVariableKey = key;
                }
                template = template.Replace(concurrentVariableKey, this.variables[key].Invoke(), StringComparison.OrdinalIgnoreCase);
            }

            return template;
        }

        private string concurrentVariableKey { get; set; }

        private Dictionary<string, Func<string>> createVariable()
        {
            return new()
            {
                { "%appname%", () => (_configuration[AppConsoleDefaults.ApplicationKey]?.ToString() ?? string.Empty) },
                { "%date%", () => DateTime.Now.ToString("yyyy-MM-dd") },
                { "%longdate%", () => DateTime.Now.ToString("yyyyMMdd") },
                { "%year%", () => DateTime.Now.ToString("yyyy") },
                { "%month%", () => DateTime.Now.ToString("MM") },
                { "%day%", () => DateTime.Now.ToString("dd") },
                { "%format:%", () => this.Handler(concurrentVariableKey) },
            };
        }

        private string Handler(string template)
        {
            var customFormats = template.Split(separatorKey)
                .Where(s => !string.IsNullOrEmpty(s))
                .ToArray();

            switch (this.RemoveEOF(customFormats[0]))
            {
                case "format":
                    return this.Formatter(customFormats);
                default:
                    return template;
            }
        }

        private string Formatter(string[] customFormats)
        {
            switch (this.RemoveEOF(customFormats[1]))
            {
                case "date":
                    return DateTime.Now.ToString(this.RemoveEOF(customFormats[2]));
                default:
                    return string.Join(separatorKey, customFormats);
            }
        }

        private string RemoveEOF(string value)
        {
            return value.Replace(EOF, string.Empty);
        }
    }
}
