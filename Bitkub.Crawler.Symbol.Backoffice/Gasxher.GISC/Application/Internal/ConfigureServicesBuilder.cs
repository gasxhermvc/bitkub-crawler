﻿using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal class ConfigureServicesBuilder
    {
        public ConfigureServicesBuilder(MethodInfo? configureServices)
        {
            MethodInfo = configureServices;
        }

        public MethodInfo? MethodInfo { get; }

        public Func<Func<IServiceCollection, IServiceProvider?>, Func<IServiceCollection, IServiceProvider?>> StartupServiceFilters { get; set; } = f => f;

        public Func<IServiceCollection, IServiceProvider?> Build(object instance) => services => Invoke(instance, services);

        private IServiceProvider? Invoke(object instance, IServiceCollection services)
        {
            return StartupServiceFilters(Startup)(services);

            IServiceProvider? Startup(IServiceCollection serviceCollection) => InvokeCore(instance, serviceCollection);
        }

        private IServiceProvider? InvokeCore(object instance, IServiceCollection services)
        {
            if (MethodInfo == null)
            {
                return null;
            }

            var parameters = MethodInfo.GetParameters();
            if (parameters.Length > 1 || parameters.Any(p => p.ParameterType != typeof(IServiceCollection)))
            {
                throw new InvalidOperationException("The ConfigureServices method must either be parameterless or take only one parameter of type IServiceCollection.");
            }

            var arguments = new object[MethodInfo.GetParameters().Length];

            if (arguments.Length > 0)
            {
                arguments[0] = services;
            }

            return MethodInfo.InvokeWithoutWrappingExceptions(instance, arguments) as IServiceProvider;
        }
    }
}
