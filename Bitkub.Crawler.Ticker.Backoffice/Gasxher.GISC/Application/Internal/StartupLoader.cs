using GasxherGIS.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GasxherGIS.Application.Internal
{
    internal class StartupLoader
    {
        // Creates an <see cref="StartupMethods"/> instance with the actions to run for configuring the application services and the
        // request pipeline of the application.
        // When using convention based startup, the process for initializing the services is as follows:
        // The host looks for a method with the signature <see cref="IServiceProvider"/> ConfigureServices(<see cref="IServiceCollection"/> services).
        // If it can't find one, it looks for a method with the signature <see cref="void"/> ConfigureServices(<see cref="IServiceCollection"/> services).
        // When the configure services method is void returning, the host builds a services configuration function that runs all the <see cref="IStartupConfigureServicesFilter"/>
        // instances registered on the host, along with the ConfigureServices method following a decorator pattern.
        // Additionally to the ConfigureServices method, the Startup class can define a <see cref="void"/> ConfigureContainer&lt;TContainerBuilder&gt;(TContainerBuilder builder)
        // method that further configures services into the container. If the ConfigureContainer method is defined, the services configuration function
        // creates a TContainerBuilder <see cref="IServiceProviderFactory{TContainerBuilder}"/> and runs all the <see cref="IStartupConfigureContainerFilter{TContainerBuilder}"/>
        // instances registered on the host, along with the ConfigureContainer method following a decorator pattern.
        // For example:
        // StartupFilter1
        //   StartupFilter2
        //     ConfigureServices
        //   StartupFilter2
        // StartupFilter1
        // ConfigureContainerFilter1
        //   ConfigureContainerFilter2
        //     ConfigureContainer
        //   ConfigureContainerFilter2
        // ConfigureContainerFilter1
        //
        // If the Startup class ConfigureServices returns an <see cref="IServiceProvider"/> and there is at least an <see cref="IStartupConfigureServicesFilter"/> registered we
        // throw as the filters can't be applied.
        public static StartupMethods LoadMethods(IServiceProvider appServiceProvider, [DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string environmentName, object? instance = null)
        {
            var configureMethod = FindConfigureDelegate(startupType, environmentName);

            var servicesMethod = FindConfigureServicesDelegate(startupType, environmentName);
            var configureContainerMethod = FindConfigureContainerDelegate(startupType, environmentName);

            if (instance == null && (!configureMethod.MethodInfo.IsStatic || (servicesMethod?.MethodInfo != null && !servicesMethod.MethodInfo.IsStatic)))
            {
                instance = ActivatorUtilities.GetServiceOrCreateInstance(appServiceProvider, startupType);
            }


            var type = configureContainerMethod.MethodInfo != null ? configureContainerMethod.GetContainerType() : typeof(object);

            var builder = (ConfigureServicesDelegateBuilder)Activator.CreateInstance(
                typeof(ConfigureServicesDelegateBuilder<>).MakeGenericType(type),
                appServiceProvider,
                servicesMethod,
                configureContainerMethod,
                instance)!;

            return new StartupMethods(instance, configureMethod.Build(instance), builder.Build());
        }

        private abstract class ConfigureServicesDelegateBuilder
        {
            public abstract Func<IServiceCollection, IServiceProvider> Build();
        }

        private class ConfigureServicesDelegateBuilder<TContainerBuilder> : ConfigureServicesDelegateBuilder where TContainerBuilder : notnull
        {
            public ConfigureServicesDelegateBuilder(
                IServiceProvider appServiceProvider,
                ConfigureServicesBuilder configureServicesBuilder,
                ConfigureContainerBuilder configureContainerBuilder,
                object instance)
            {
                AppServiceProvider = appServiceProvider;
                ConfigureServiceBuilder = configureServicesBuilder;
                ConfigureContainerBuilder = configureContainerBuilder;
                Instance = instance;
            }

            public IServiceProvider AppServiceProvider { get; }
            public ConfigureServicesBuilder ConfigureServiceBuilder { get; }
            public ConfigureContainerBuilder ConfigureContainerBuilder { get; }
            public object Instance { get; }

            public override Func<IServiceCollection, IServiceProvider> Build()
            {
                ConfigureServiceBuilder.StartupServiceFilters = BuildStartupServicesFilterPipeline;
                var configureServiceCallback = ConfigureServiceBuilder.Build(Instance);

                ConfigureContainerBuilder.ConfigureContainerFilters = ConfigureContainerPipeline;
                var configureContainerCallback = ConfigureContainerBuilder.Build(Instance);

                return ConfigureServices(configureServiceCallback, configureContainerCallback);

                Action<object> ConfigureContainerPipeline(Action<object> action)
                {
                    return Target;

                    void Source(TContainerBuilder containerBuilder) => action(containerBuilder);

                    void Target(object containerBuilder) => BuildStartupConfigureContainerFiltersPipeline(Source)((TContainerBuilder)containerBuilder);

                }
            }

            Func<IServiceCollection, IServiceProvider> ConfigureServices(
                Func<IServiceCollection, IServiceProvider?> configureServicesCallback,
                Action<object> configureContainerCallback)
            {
                return ConfigureServicesWithContainerConfiguration;

                IServiceProvider ConfigureServicesWithContainerConfiguration(IServiceCollection services)
                {
                    var applicationServiceProvider = configureServicesCallback.Invoke(services);

                    if (applicationServiceProvider != null)
                    {
                        return applicationServiceProvider;
                    }

                    if (ConfigureContainerBuilder.MethodInfo != null)
                    {
                        var serviceProviderFactory = AppServiceProvider.GetRequiredService<IServiceProviderFactory<TContainerBuilder>>();
                        var builder = serviceProviderFactory.CreateBuilder(services);
                        configureContainerCallback(builder);
                        applicationServiceProvider = serviceProviderFactory.CreateServiceProvider(builder);
                    }
                    else
                    {
                        var serviceProviderFactory = AppServiceProvider.GetRequiredService<IServiceProviderFactory<IServiceCollection>>();
                        var builder = serviceProviderFactory.CreateBuilder(services);
                        applicationServiceProvider = serviceProviderFactory.CreateServiceProvider(builder);
                    }

                    return applicationServiceProvider ?? services.BuildServiceProvider();
                }
            }

            private Func<IServiceCollection, IServiceProvider?> BuildStartupServicesFilterPipeline(Func<IServiceCollection, IServiceProvider?> startup)
            {
                return RunPipeline;

                IServiceProvider? RunPipeline(IServiceCollection services)
                {
                    var filters = AppServiceProvider.GetRequiredService<IEnumerable<IStartupConfigureServicesFilter>>()
                        .ToArray();

                    if (filters.Length == 0)
                    {
                        return startup(services);
                    }

                    Action<IServiceCollection> pipeline = InvokeStartup;
                    for (int i = filters.Length - 1; i >= 0; i--)
                    {
                        pipeline = filters[i].ConfigureServices(pipeline);
                    }

                    pipeline(services);

                    return null;

                    void InvokeStartup(IServiceCollection serviceCollection)
                    {
                        var result = startup(serviceCollection);
                        if (filters.Length > 0 && result != null)
                        {
                            var message = $"A ConfigureServices method that returns an {nameof(IServiceProvider)} is " +
                                $"not compatible with the use of one or more {nameof(IStartupConfigureServicesFilter)}. " +
                                $"Use a void returning ConfigureServices method instead or a ConfigureContainer method.";

                            throw new InvalidOperationException(message);
                        }
                    }
                }
            }

            private Action<TContainerBuilder> BuildStartupConfigureContainerFiltersPipeline(Action<TContainerBuilder> configureContainer)
            {
                return RunPipeline;

                void RunPipeline(TContainerBuilder containerBuilder)
                {
                    var filters = AppServiceProvider
                        .GetRequiredService<IEnumerable<IStartupConfigureContainerFilter<TContainerBuilder>>>();

                    Action<TContainerBuilder> pipeline = InvokeConfigureContainer;
                    foreach (var filter in filters.Reverse())
                    {
                        pipeline = filter.ConfigureContainer(pipeline);
                    }

                    pipeline(containerBuilder);

                    void InvokeConfigureContainer(TContainerBuilder builder) => configureContainer(builder);
                }
            }
        }

        [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode", Justification = "We're warning at the entry point. This is an implementation detail.")]
        public static Type FindStartupType(string startupAssemblyName, string environmentName)
        {
            if (string.IsNullOrEmpty(startupAssemblyName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        "A startup method, startup type or startup assembly is required. If specifying an assembly, '{0}' cannot be null or empty.",
                        nameof(startupAssemblyName)),
                        nameof(startupAssemblyName));
            }

            var assembly = Assembly.Load(new AssemblyName(startupAssemblyName));
            if (assembly == null)
            {
                throw new InvalidOperationException($"The assembly '{startupAssemblyName}' failed to load.");
            }

            var startupNameWithEnv = "Startup" + environmentName;
            var startupNameWithoutEnv = "Startup";

            // Check the most likely places first
            var type =
                assembly.GetType(startupNameWithEnv) ??
                assembly.GetType(startupAssemblyName + "." + startupNameWithEnv) ??
                assembly.GetType(startupNameWithoutEnv) ??
                assembly.GetType(startupAssemblyName + "." + startupNameWithoutEnv);

            if (type == null)
            {
                // Full scan
                var definedTypes = assembly.DefinedTypes.ToList();

                var startupType1 = definedTypes.Where(info => info.Name.Equals(startupNameWithEnv, StringComparison.OrdinalIgnoreCase));
                var startupType2 = definedTypes.Where(info => info.Name.Equals(startupNameWithoutEnv, StringComparison.OrdinalIgnoreCase));

                var typeInfo = startupType1.Concat(startupType2).FirstOrDefault();
                if (typeInfo != null)
                {
                    type = typeInfo.AsType();
                }
            }

            if (type == null)
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "A type named '{0}' or '{1}' could not be found in assembly '{2}'.",
                    startupNameWithEnv,
                    startupNameWithoutEnv,
                    startupAssemblyName));
            }

            return type;
        }

        internal static ConfigureBuilder FindConfigureDelegate([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string environmentName)
        {
            var configureMethod = FindMethod(startupType, "Configure{0}", environmentName, typeof(void), required: true);
            return new ConfigureBuilder(configureMethod);
        }

        internal static ConfigureContainerBuilder FindConfigureContainerDelegate([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string environmentName)
        {
            var configureMethod = FindMethod(startupType, "Configure{0}Container", environmentName, typeof(void), required: false);

            return new ConfigureContainerBuilder(configureMethod);
        }

        internal static bool HasConfigureServicesIServiceProviderDelegate([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string environmentName)
        {
            return null != FindMethod(startupType, "Configure{0}Services", environmentName, typeof(IServiceProvider), required: false);
        }

        internal static ConfigureServicesBuilder FindConfigureServicesDelegate([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string environmentName)
        {
            var servicesMethod = FindMethod(startupType, "Configure{0}Services", environmentName, typeof(IServiceProvider), required: false)
                ?? FindMethod(startupType, "Configure{0}Services", environmentName, typeof(void), required: false);


            return new ConfigureServicesBuilder(servicesMethod);
        }

        private static MethodInfo? FindMethod([DynamicallyAccessedMembers(StartupLinkerOptions.Accessibility)] Type startupType, string methodName, string environmentName, Type? returnType = null, bool required = true)
        {
            var methodNameWithEnv = string.Format(CultureInfo.InvariantCulture, methodName, environmentName);
            var methodNameWithNoEnv = string.Format(CultureInfo.InvariantCulture, methodName, "");

            var methods = startupType.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            var selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithEnv, StringComparison.OrdinalIgnoreCase)).ToList();
            if (selectedMethods.Count > 1)
            {
                throw new InvalidOperationException($"Having multiple overloads of method '{methodNameWithEnv}' is not supported.");
            }

            if (selectedMethods.Count == 0)
            {
                selectedMethods = methods.Where(method => method.Name.Equals(methodNameWithNoEnv, StringComparison.OrdinalIgnoreCase)).ToList();
                if (selectedMethods.Count > 1)
                {
                    throw new InvalidOperationException($"Having multiple overloads of method '{methodNameWithEnv}' is not supported.");
                }
            }

            var methodInfo = selectedMethods.FirstOrDefault();
            if (methodInfo == null)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "A public method named '{0}' or '{1}' could not be found in the '{2}' type.",
                        methodNameWithEnv,
                        methodNameWithNoEnv,
                        startupType.FullName));
                }
                return null;
            }

            if (returnType != null && methodInfo.ReturnType != returnType)
            {
                if (required)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "The '{0}' method in the type '{1}' must have a return type of '{2}'.",
                        methodInfo.Name,
                        startupType.FullName,
                        returnType.Name));
                }
                return null;
            }

            return methodInfo;
        }
    }
}
