using Microsoft.Extensions.Configuration;

namespace GasxherGIS.Standards.Environment
{
    public interface IAppEnvironment
    {
        string ApplicationName { get; set; }

        string AppRootPath { get; set; }

        string ContentRootPath { get; set; }

        void SetEnvironment(string environmentName);

        IConfiguration SetEnvironment(string environmentName, IConfiguration configuration);

        string GetEnvironment();

        bool IsDevelopment();

        bool IsInhouse();

        bool IsNonProduction();

        bool IsProduction();

        bool IsEnvironment(string environment);

        T GetEnvironmentVariable<T>(string environmentVariableName);

        string GetEnvironmentVariable(string environmentVariableName);
    }
}
