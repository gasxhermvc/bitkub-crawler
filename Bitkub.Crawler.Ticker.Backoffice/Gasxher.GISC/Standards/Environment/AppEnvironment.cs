using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using GasxherGIS.Application;
using GasxherGIS.Standards.Command.Internal;
using GasxherGIS.Application.Internal;
using System.IO;

namespace GasxherGIS.Standards.Environment
{
    public class AppEnvironment : IAppEnvironment
    {
        private readonly ICommandParser _parser;
        public string Environment { get; set; } = default;

        public string ApplicationName { get; set; } = default;

        public string AppRootPath { get; set; } = default!;

        public string ContentRootPath { get; set; } = default!;


        private const string DevelopmentMode = "Development";
        private const string InhouseMode = "Inhouse";
        private const string NonProductionMode = "NonProduction";
        private const string ProductionMode = "Production";

        private Dictionary<string, string> defaultEnv = new Dictionary<string, string>()
        {
            { "ASPNETCORE_ENVIRONMENT","Development" },
            { "EnvironmentName","Development" },
            { "EnvironmentVariable","Development" },
        };

        /// <summary>
        /// สร้าง Instance Class และ Inject Class <see cref="ICommandParser"/> เพื่อใช้ในการดึงข้อมูล Environment
        /// </summary>
        /// <param name="parser"></param>
        public AppEnvironment(ICommandParser parser)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }

            _parser = parser;

            //=>เริ่มการอีพเดตในครั้งแรกที่ Application กำลังสร้างตัวเอง
            this.UpdateAppEnvironment();
        }

        public void Initialize(string contentRootPath, AppConsoleOptions options)
        {
            ApplicationName = options.ApplicationName;
            ContentRootPath = contentRootPath;
            if (options.AppRoot == null)
            {
                AppRootPath = ContentRootPath;
            }
            options.AppRoot = AppRootPath;
            options.ContentRootPath = ContentRootPath;


            Environment = options?.Environment ?? this.GetEnvironment();
            options.Environment = Environment;
        }

        /// <summary>
        /// กำหนด Environment ของระบบ
        /// </summary>
        /// <param name="environmentName"></param>
        public void SetEnvironment(string environmentName)
        {
            foreach (var key in defaultEnv.Keys)
            {
                this.defaultEnv[key] = environmentName;
            }

            this.UpdateAppEnvironment(environmentName);
        }

        /// <summary>
        /// กำหนด Environment ของระบบ พร้อมรีโหลด Configuration
        /// </summary>
        /// <param name="environmentName"></param>
        /// <param name="configuration"></param>
        public IConfiguration SetEnvironment(string environmentName, IConfiguration configuration)
        {
            this.SetEnvironment(environmentName);
            //return configuration.ReloadConfig(environmentName);
            return configuration;
        }

        /// <summary>
        /// ดึงค่า Environment ของระบบ
        /// </summary>
        /// <returns></returns>
        public string GetEnvironment()
        {
            return this.Environment;
        }

        /// <summary>
        /// Environment ปัจจุบันเท่ากับ Development
        /// </summary>
        /// <returns></returns>
        public bool IsDevelopment()
        {
            return string.Equals(this.Environment, AppEnvironment.DevelopmentMode
                , StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Environment ปัจจุบันเท่ากับ Inhouse
        /// </summary>
        /// <returns></returns>
        public bool IsInhouse()
        {
            return string.Equals(this.Environment, AppEnvironment.InhouseMode
                , StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Environment ปัจจุบันเท่ากับ Non Production
        /// </summary>
        /// <returns></returns>
        public bool IsNonProduction()
        {
            return string.Equals(this.Environment, AppEnvironment.NonProductionMode
                , StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Environment ปัจจุบันเท่ากับ Production
        /// </summary>
        /// <returns></returns>
        public bool IsProduction()
        {
            return string.Equals(this.Environment, AppEnvironment.ProductionMode
                , StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Environment ปัจจุบันเท่ากับ ค่าใดๆ
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public bool IsEnvironment(string environment)
        {
            return string.Equals(this.Environment, environment
                , StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// ดึงข้อมูลจาก Environment Variable ที่สร้างพร้อม Generic Object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="envName"></param>
        /// <returns></returns>
        public T GetEnvironmentVariable<T>(string environmentVariableName)
        {
            var environmentVariable = this.GetEnvironmentVariable(environmentVariableName);

            if (environmentVariable == null || string.IsNullOrEmpty(environmentVariable)) return (T)default;

            try
            {
                //=>Parser ข้อความ ตัวเลข บลูลีน
                return (T)Convert.ChangeType(environmentVariable, typeof(T));
            }
            catch
            {
                //=>Parser object หรือ array
                return (T)JsonConvert.DeserializeObject(environmentVariable, typeof(T));
            }
        }

        /// <summary>
        /// ดึงข้อมูลจาก Environment Variable กลับมาเป็นข้อความ
        /// </summary>
        /// <param name="envName"></param>
        /// <returns></returns>
        public string GetEnvironmentVariable(string environmentVariableName)
        {
            var environmentVariable = System.Environment.GetEnvironmentVariable(environmentVariableName);

            if (string.IsNullOrEmpty(environmentVariable))
            {
                return this.defaultEnv.TryGetValue(environmentVariableName, out environmentVariable) ? environmentVariable : string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// ค้นหา Environment ของระบบจาก Command Arguments
        /// </summary>
        /// <returns></returns>
        private string FromCommandLineArguments()
        {
            string environmentName = string.Empty;

            foreach (var key in this.defaultEnv.Keys)
            {
                var environmentVariable = _parser.GetValue<string>(key);

                if (!string.IsNullOrEmpty(environmentVariable))
                {
                    environmentName = environmentVariable;
                    break;
                }
            }

            return environmentName;
        }

        /// <summary>
        /// อัพเดตค่า Environment สำหรับกำหนดภายใน App และ จาก CommandLine Arguments
        /// </summary>
        /// <param name="environment"></param>
        private void Update(string environmentVariable)
        {
            //=>เมื่อมีการกำหนด Environment ในการ Deploy app ให้ทำการ override ทับด้วย
            foreach (var key in defaultEnv.Keys)
            {
                defaultEnv[key] = environmentVariable;
            }

            this.Environment = environmentVariable;
        }

        /// <summary>
        /// อัพเดตค่า Environment
        /// </summary>
        private void UpdateAppEnvironment(string internalEnvironment = "")
        {
            //=>Internal Update
            //=>Priority High: อัพเดตค่า Environment จากภายใน Application
            if (!string.IsNullOrEmpty(internalEnvironment))
            {
                //=>เมื่อมีการกำหนด Environment ในการ Deploy app ให้ทำการ override ทับด้วย
                this.Update(internalEnvironment);
                return;
            }

            //=>Priority Middle: อัพเดตค่า Environment จาก Argument ที่ส่งเข้ามาตอนรันแอปพลิเคชัน
            string argumentEnvironment = this.FromCommandLineArguments();
            if (!string.IsNullOrEmpty(argumentEnvironment))
            {
                //=>เมื่อมีการกำหนด Environment ในการ Deploy app ให้ทำการ override ทับด้วย
                this.Update(argumentEnvironment);
                return;
            }

            //=>Priority Low: ค้นหาค่า Environment จาก System Environment Variable
            var systemEnvironmentVariable = string.Empty;
            foreach (var key in defaultEnv.Keys)
            {
                //=>เมื่อพบ Environment แล้วจะทำการอัพเดตค่าอื่นๆ ให้เป็นค่าเดียวกัน
                if (!string.IsNullOrEmpty(systemEnvironmentVariable))
                {
                    defaultEnv[key] = systemEnvironmentVariable;
                    continue;
                }

                var systemEnv = System.Environment.GetEnvironmentVariable(key);

                if (!string.IsNullOrEmpty(systemEnv) && this.defaultEnv.ContainsKey(key))
                {
                    systemEnvironmentVariable = systemEnv;
                    defaultEnv[key] = systemEnv;
                }
            }

            this.Environment = defaultEnv[AppConsoleDefaults.EnvironmentKey];
            return;

        }
    }
}
