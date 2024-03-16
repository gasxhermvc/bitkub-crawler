using GasxherGIS.Application.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application
{
    internal interface ISupportsStartup
    {
        /// <summary>
        /// กำหนด Confiure ซึ่งจะใช้เป็นส่วนของการตรวจสอบแบบ Chain Responsibility Pattern
        /// </summary>
        /// <param name="configure"></param>
        /// <returns></returns>
        IConsoleBuilder Configure(Action<ConsoleBuilderContext, IApplicationBuilder> configure);


        /// <summary>
        /// กำหนดใช้ Startup เพื่อกำหนด Injection และการตรวจสอบแบบ Chain Responsibility Pattern
        /// </summary>
        /// <param name="startupType"></param>
        /// <returns></returns>
        IConsoleBuilder UseStartup([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType);


        /// <summary>
        /// กำหนดใช้ Startup เพื่อกำหนด Injection และการตรวจสอบแบบ Chain Responsibility Pattern จาก GenericType Startup
        /// </summary>
        /// <typeparam name="TStartup"></typeparam>
        /// <param name="startupFactory"></param>
        /// <returns></returns>
        IConsoleBuilder UseStartup<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods)]TStartup>(Func<ConsoleBuilderContext, TStartup> startupFactory);
    }
}
