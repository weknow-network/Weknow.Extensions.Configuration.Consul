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
    internal sealed class ConsulConfigurationProvider : ConfigurationProvider, IAsyncDisposable
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConcurrentDictionary<string, string> _rawConfiguration =
                                                new ConcurrentDictionary<string, string>();
        private readonly ConcurrentDictionary<string, string> _deserializedConfiguration =
                                                new ConcurrentDictionary<string, string>();

        private readonly IConsulHierarchy _hierarchic;
        private readonly IConsulProxy _factory;

        //private ulong _lastIndex;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulConfigurationProvider"/> class.
        /// </summary>
        /// <param name="hierarchic">The hierarchic.</param>
        /// <param name="factory">The factory.</param>
        public ConsulConfigurationProvider(
            IConsulHierarchy hierarchic,
            IConsulProxy factory)
        {
            _hierarchic = hierarchic;
            _factory = factory;
        }

        #endregion // Ctor

        #region DisposeAsync

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous dispose operation.
        /// </returns>
        public ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            return new ValueTask();
        }

        #endregion // DisposeAsync

        public override void Load()
        {

        }

        public override bool TryGet(string key, out string value)
        {
            value = "Test 123";
            return true;
        }
    }
}