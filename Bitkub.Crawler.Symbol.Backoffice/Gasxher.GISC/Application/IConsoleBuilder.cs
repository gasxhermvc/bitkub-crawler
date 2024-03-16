using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GasxherGIS.Application
{
    public interface IConsoleBuilder
    {
        #region Wait Fix ไม่สามารถอีพเดตค่าเดิมได้
        /// <summary>
        /// กำหนด Configuration ของ Application
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        //IConsoleBuilder ConfigureAppConfiguration(Action<IConfiguration> configureDelegate);


        ///// <summary>
        ///// กำหนด Configuration ของ Application และสามารถนำ ConsoleBuilderContext มาใช้งานได้
        ///// </summary>
        ///// <param name="configureDelegate"></param>
        ///// <returns></returns>
        //IConsoleBuilder ConfigureAppConfiguration(Action<ConsoleBuilderContext, IConfiguration> configureDelegate);
        #endregion

        /// <summary>
        /// กำหนด Injection ของ Application และสามารถนำ ConsoleBuilderContext มาใช้งานได้
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        IConsoleBuilder ConfigureServices(Action<ConsoleBuilderContext, IServiceCollection> configureDelegate);



        /// <summary>
        /// กำหนด Injection ของ Application
        /// </summary>
        /// <param name="configureServices"></param>
        /// <returns></returns>
        IConsoleBuilder ConfigureServices(Action<IServiceCollection> configureServices);

        /// <summary>
        /// Run Application เพื่อเริ่มต้นการทำงานของ Application
        /// </summary>
        /// <returns></returns>
        IConsole Build();


        /// <summary>
        /// Run Application เพื่อเริ่มต้นการทำงานของ Application และกำหนด Configures เพื่อ Injection
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <returns></returns>
        IConsole Build<TStartup>();

        string GetSetting(string key);

        IConsoleBuilder UseSetting(string key, string? value);
    }
}
