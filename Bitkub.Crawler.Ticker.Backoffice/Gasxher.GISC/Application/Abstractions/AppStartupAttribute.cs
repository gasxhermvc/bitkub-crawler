using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Abstractions
{
    [AttributeUsage(AttributeTargets.Assembly, Inherited = false, AllowMultiple = true)]
    public class AppStartupAttribute : Attribute
    {
        public AppStartupAttribute([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] Type appStartupType)
        {
            if (appStartupType == null)
            {
                throw new ArgumentNullException(nameof(AppStartupAttribute));
            }

            if (!typeof(IAppStartup).IsAssignableFrom(appStartupType))
            {
                throw new ArgumentException($@"""{appStartupType}"" does not implement {typeof(IAppStartup)}.", nameof(appStartupType));
            }

            AppStartupType = appStartupType;
        }

        /// <summary>
        /// The implementation of <see cref="IHostingStartup"/> that should be loaded when
        /// starting an application.
        /// </summary>
        [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)]
        public Type AppStartupType { get; }
    }
}
