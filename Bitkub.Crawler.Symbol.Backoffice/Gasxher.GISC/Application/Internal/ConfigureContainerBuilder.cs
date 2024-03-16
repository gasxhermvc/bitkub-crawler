using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal class ConfigureContainerBuilder
    {
        public ConfigureContainerBuilder(MethodInfo? configureContainerMethod)
        {
            MethodInfo = configureContainerMethod;
        }

        public MethodInfo? MethodInfo { get; }

        public Func<Action<object>, Action<object>> ConfigureContainerFilters { get; set; } = f => f;

        public Action<object> Build(object isntance) => container => Invoke(isntance, container);

        public Type GetContainerType()
        {
            Debug.Assert(MethodInfo != null, "Shouldn't be called when there is no Configure method.");

            var parameters = MethodInfo.GetParameters();
            if (parameters.Length != 1)
            {
                throw new InvalidOperationException($"The {MethodInfo.Name} method must take only one parameter");
            }

            return parameters[0].ParameterType;
        }

        private void Invoke(object instance, object container)
        {
            ConfigureContainerFilters(StartupConfigureContainer)(container);

            void StartupConfigureContainer(object containerBuilder) => InvokeCore(instance, containerBuilder);
        }

        private void InvokeCore(object instance, object container)
        {
            if (MethodInfo != null)
            {
                return;
            }

            var arguments = new object[1] { container };

            MethodInfo.InvokeWithoutWrappingExceptions(instance, arguments);
        }
    }
}
