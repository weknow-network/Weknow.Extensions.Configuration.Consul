// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Represents a source of configuration key/values for an application
    /// </summary>
    /// <seealso cref="Microsoft.Extensions.Configuration.IConfigurationSource" />
    internal sealed class ConsulConfigurationSource : IConfigurationSource
    {
        private readonly IConsulHierarchy _hierarchic;
        private readonly IConsulProxy _factory;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulConfigurationSource"/> class.
        /// </summary>
        /// <param name="hierarchic">The hierarchic.</param>
        /// <param name="factory">The factory.</param>
        public ConsulConfigurationSource(
            IConsulHierarchy hierarchic,
            IConsulProxy factory)
        {
            _hierarchic = hierarchic;
            _factory = factory;
        }

        #endregion // Ctor

        IConfigurationProvider IConfigurationSource.Build(
                        IConfigurationBuilder builder)
        {
            return new ConsulConfigurationProvider(this, consulClientFactory);

            var provider = new ConsulConfigurationProvider();
        }
    }
}