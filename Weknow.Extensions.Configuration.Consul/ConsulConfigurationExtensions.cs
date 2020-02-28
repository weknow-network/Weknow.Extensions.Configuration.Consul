// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Weknow.Extensions.Configuration.Consul;

namespace Microsoft.Extensions.Hosting
{
    public static class ConsulConfigurationExtensions
    {
        /// <summary>
        /// Adds the consul configuration.
        /// </summary>
        /// <param name="hostBuilder">The host builder.</param>
        /// <param name="convensionSetting">
        /// Convention builder for setting the Consul hierarchic structure.
        /// Default convention is: Application, Environment, Component</param>
        /// <param name="address">Optional address of the console service.
        /// If missing will check:
        /// * Environment variable: consul-url
        /// * default (if don't find anything): http://127.0.0.1:8500
        /// .</param>
        /// <param name="configOverride">Enable to override configuration.</param>
        /// <param name="httpClientOverride">Enable to override HTTP client.</param>
        /// <param name="httpClientHandlerOverride">Enable to override HTTP client handler.</param>
        /// <returns></returns>
        public static IHostBuilder AddConsulConfiguration(
            this IHostBuilder hostBuilder,
            Func<IConsulHierarchyBuilder, IConsulHierarchy> hierarchyConventionSetting,
            string? address = null,
            Action<ConsulClientConfiguration>? configOverride = null,
            Action<HttpClient>? httpClientOverride = null,
            Action<HttpClientHandler>? httpClientHandlerOverride = null)
        {
            #region IConsulHierarchy hierarchyConvention = ...

            var hierarchyBuilder = ConsulHierarchyBuilder.Default;
            IConsulHierarchy hierarchyConvention =
                hierarchyConventionSetting?.Invoke(hierarchyBuilder) ??
                hierarchyBuilder.ByAppName()
                                .ByEnvironment()
                                .ByComponent()

                                .Build();
            #endregion // IConsulHierarchy hierarchyConvention = ...
            
            var options = new ConsulClientOptions(address, configOverride, httpClientOverride, httpClientHandlerOverride);
            IConsulProxy factory = new ConsulClientFactory(options);
            hostBuilder.ConfigureServices((hostContext, services) =>
            {
                services.AddSingleton(factory);
            });

            hostBuilder = hostBuilder.ConfigureAppConfiguration(
                     (host, builder) =>
                     {
                         // string consoleUrl = host.Configuration["Consul:Host"];
                         var env = host.HostingEnvironment;
                         builder.SetBasePath(env.ContentRootPath);

                         var configSource = new ConsulConfigurationSource(hierarchyConvention, factory);
                         builder.Add(configSource);

                         builder.AddEnvironmentVariables();
                     });
            return hostBuilder;
        }

    }
}