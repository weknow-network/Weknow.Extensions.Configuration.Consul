// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Consul;
using Microsoft.Extensions.Configuration;
using static Weknow.Extensions.Configuration.Consul.JsonUtils;
using static System.Text.Encoding;
using static Weknow.Extensions.Configuration.Consul.HierarchicConsts;
using System.Diagnostics;

namespace Weknow.Extensions.Configuration.Consul
{
    internal sealed class ConsulConfigurationProvider : ConfigurationProvider, IAsyncDisposable
    {
        private static readonly TimeSpan LOCK_TIMEOT = TimeSpan.FromSeconds(30);
        private readonly AsyncLock _gate = new AsyncLock(LOCK_TIMEOT);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConsulConfigurationData _data;

        private readonly IConsulHierarchy _hierarchic;
        private readonly IConsulProxy _proxy;
        private TaskCompletionSource<object?> _initializing = new TaskCompletionSource<object?>();

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulConfigurationProvider"/> class.
        /// </summary>
        /// <param name="hierarchic">The hierarchic.</param>
        /// <param name="proxy">The Consul's proxy.</param>
        public ConsulConfigurationProvider(
            IConsulHierarchy hierarchic,
            IConsulProxy proxy)
        {
            _hierarchic = hierarchic;
            _proxy = proxy;
            _data = new ConsulConfigurationData(hierarchic);
        }

        #endregion // Ctor

        #region DisposeAsync

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources asynchronously.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous dispose operation.
        /// </returns>
        public async ValueTask DisposeAsync()
        {
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            await _proxy.DisposeAsync();
        }

        #endregion // DisposeAsync

        #region Load

        /// <summary>
        /// Loads (or reloads) the data for this provider.
        /// </summary>
        public override void Load()
        {
            Task _ = LoadAsync();
        }

        #endregion // Load

        #region LoadAsync


        /// <summary>
        /// Initializing or reloading.
        /// </summary>
        /// <returns></returns>
        private async Task LoadAsync()
        {
            _cancellationTokenSource.Token.ThrowIfCancellationRequested();
            try
            {
                var lockScope = await _gate.TryAcquireAsync();
                if (!lockScope.Acquired)
                    return;

                string root = _hierarchic.Root;
                KVPair[] results = await _proxy.GetDataAsync(root, _cancellationTokenSource.Token);
                foreach (KVPair pair in results)
                {
                    string key = pair.Key.ToLower();
                    byte[] value = pair.Value;
                    if (!string.IsNullOrEmpty(root))
                        key = key.Substring(root.Length + 1);
                    _data.AddOrUpdate(key, value);
                }

                _initializing.TrySetResult(null);
            }
            catch (Exception ex)
            {
                _initializing.TrySetException(ex);
            }
        }

        #endregion // LoadAsync

        #region TryGet

        /// <summary>
        /// Attempts to find a value with the given key, returns true if one is found, false otherwise.
        /// </summary>
        /// <param name="key">The key to lookup.</param>
        /// <param name="value">The value found at key if one is found.</param>
        /// <returns>
        /// True if key has a value, false otherwise.
        /// </returns>
        public override bool TryGet(string key, out string value)
        {
            #region _initializing.Task.Wait(...)

            var token = CancellationTokenSource.CreateLinkedTokenSource(
                        _cancellationTokenSource.Token,
                        new CancellationTokenSource(LOCK_TIMEOT).Token
                    );
            _initializing.Task.Wait(token.Token);

            #endregion // _initializing.Task.Wait(...)

            value = _data.GetAddMergedValue(key);

            return value != string.Empty;
        }

        #endregion // TryGet
    }
}