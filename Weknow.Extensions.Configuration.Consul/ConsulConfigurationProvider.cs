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
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Primitives;

namespace Weknow.Extensions.Configuration.Consul
{
    internal sealed class ConsulConfigurationProvider :
        IConfigurationProvider,
        IAsyncDisposable
    {
        private ConfigurationReloadToken _reloadToken = new ConfigurationReloadToken();

        private static readonly TimeSpan LOAD_TIMEOT = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan LOCK_TIMEOT = TimeSpan.FromSeconds(30);
        private readonly AsyncLock _gate = new AsyncLock(LOCK_TIMEOT);

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ConsulConfigurationData _data;

        private readonly IConsulHierarchy _hierarchic;
        private readonly IConsulProxy _proxy;

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
        /// Loads configuration values from the source represented by this <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />.
        /// </summary>
        void IConfigurationProvider.Load()
        {
            LoadAsync(true).Wait(LOAD_TIMEOT);
        }

        #endregion // Load

        #region LoadAsync

        /// <summary>
        /// Initializing or reloading.
        /// </summary>
        /// <returns></returns>
        private async Task LoadAsync(bool init = false)
        {
            var lockScope = await _gate.TryAcquireAsync();
            if (!lockScope.Acquired)
                return;

            string root = _hierarchic.Root;
            KVPair[] results = await _proxy.GetDataAsync(root, _cancellationTokenSource.Token);
            Array.Sort(results, KVPairComparer.Default);
            foreach (KVPair pair in results)
            {
                string key = pair.Key.ToLower();
                byte[] value = pair.Value;
                if (!string.IsNullOrEmpty(root))
                    key = key.Substring(root.Length + 1);
                _data.AddOrUpdate(key, value);
            }

            OnReload();
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
        bool IConfigurationProvider.TryGet(string key, out string value)
        {
            value = _data.GetAddMergedValue(key);

            return value != string.Empty;
        }

        #endregion // TryGet

        #region GetReloadToken

        /// <summary>
        /// Returns a change token if this provider supports change tracking, null otherwise.
        /// </summary>
        /// <returns>
        /// The change token.
        /// </returns>
        IChangeToken IConfigurationProvider.GetReloadToken() => _reloadToken;

        #endregion // GetReloadToken

        #region OnReload

        /// <summary>
        /// Triggers the reload change token and creates a new one.
        /// </summary>
        private void OnReload()
        {
            var previousToken = Interlocked.Exchange(ref _reloadToken, new ConfigurationReloadToken());
            previousToken.OnReload();
        }

        #endregion // OnReload

        #region Set

        /// <summary>
        /// Sets a configuration value for the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="NotImplementedException"></exception>
        void IConfigurationProvider.Set(string key, string value)
        {
            throw new NotImplementedException();
        }

        #endregion // Set

        #region GetChildKeys

        /// <summary>
        /// Returns the immediate descendant configuration keys for a given parent path based on this
        /// <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />s data and the set of keys returned by all the preceding
        /// <see cref="T:Microsoft.Extensions.Configuration.IConfigurationProvider" />s.
        /// </summary>
        /// <param name="earlierKeys">The child keys returned by the preceding providers for the same parent path.</param>
        /// <param name="parentPath">The parent path.</param>
        /// <returns>
        /// The child keys.
        /// </returns>
        /// <exception cref="NotImplementedException"></exception>
        IEnumerable<string> IConfigurationProvider.GetChildKeys(IEnumerable<string> earlierKeys, string parentPath)
        {
            var result = earlierKeys;
            //var prefix = parentPath == null ? string.Empty : parentPath + ConfigurationPath.KeyDelimiter;

            //var result = _data
            //    .Where(kv => kv.Key.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            //    .Select(kv => Segment(kv.Key, prefix.Length))
            //    .Concat(earlierKeys)
            //    .OrderBy(k => k, ConfigurationKeyComparer.Instance)
            //    .ToArray();

            return result;

            ///// <summary>
            ///// Segments the specified key.
            ///// </summary>
            ///// <param name="key">The key.</param>
            ///// <param name="prefixLength">Length of the prefix.</param>
            ///// <returns></returns>
            //string Segment(string key, int prefixLength)
            //{
            //    var indexOf = key.IndexOf(ConfigurationPath.KeyDelimiter, prefixLength, StringComparison.OrdinalIgnoreCase);
            //    return indexOf < 0 ? key.Substring(prefixLength) : key.Substring(prefixLength, indexOf - prefixLength);
            //}
        }

        #endregion // GetChildKeys
    }
}