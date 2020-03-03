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
using System.Collections;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Configuration data representation
    /// </summary>
    internal sealed class ConsulConfigurationData: IDictionary<string, string>
    {
        private readonly ConcurrentDictionary<string, byte[]> _rawConfiguration =
                                                new ConcurrentDictionary<string, byte[]>();
        private readonly ConcurrentDictionary<string, string> _mergedConfiguration =
                                                new ConcurrentDictionary<string, string>();

        private readonly IConsulHierarchy _hierarchic;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulConfigurationData"/> class.
        /// </summary>
        /// <param name="hierarchic">The hierarchic.</param>
        public ConsulConfigurationData(
            IConsulHierarchy hierarchic)
        {
            _hierarchic = hierarchic;
        }

        #endregion // Ctor

        #region GetAddMergedValue

        /// <summary>
        /// Gets the add merged value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public string GetAddMergedValue(string key)
        {
            string json = string.Empty;
            byte[] buffer = Array.Empty<byte>();
            MergeSubPath(key, ref json, ref buffer);

            var path = new StringBuilder();
            foreach (var hierarchic in _hierarchic.Path)
            {
                string h = hierarchic switch
                {
                    ENV_KEY => _hierarchic.DeploymentEnvironment,
                    APP_KEY => _hierarchic.ApplicationName,
                    NS_KEY => throw new NotImplementedException(),
                    COMP_KEY => throw new NotImplementedException(),
                    _ => throw new NotSupportedException(hierarchic)
                };
                if (path.Length != 0)
                    path.Append("/");
                path.Append(h);
                MergeSubPath($"{path}/{key}", ref json, ref buffer);
            }
            return json;
        }

        #endregion // GetAddMergedValue

        #region MergeSubPath

        /// <summary>
        /// Merges the sub path.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="json">The json.</param>
        /// <param name="buffer">The buffer.</param>
        private void MergeSubPath(
                    string key,
                    ref string json,
                    ref byte[] buffer)
        {
            key = key.ToLower();
            bool isCached = _mergedConfiguration.TryGetValue(key, out string? newJson);
            if (isCached)
                json = newJson ?? string.Empty;
            if (_rawConfiguration.TryGetValue(key, out byte[]? newBuffer))
            {
                if (!isCached)
                {
                    if (buffer.Length == 0)
                        json = UTF8.GetString(newBuffer);
                    else
                        json = MergeAsString(buffer, newBuffer);
                    _mergedConfiguration.TryAdd(key, json);
                }
                buffer = newBuffer;
            }
        }

        #endregion // MergeSubPath

        #region AddOrUpdate

        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        public void AddOrUpdate(string key, byte[] value)
        {
            _rawConfiguration.AddOrUpdate(key, value, (k, v) => value);

            foreach (string removalKey in _rawConfiguration.Keys
                                        .Where(k => k.StartsWith(key)))
            {
                _mergedConfiguration.TryRemove(removalKey, out string? val);
            }
            
        }

        /// <summary>
        /// Adds the or update.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="json">The json.</param>
        public void AddOrUpdate(string key, string json)
        {
            _mergedConfiguration.TryAdd(key, json);
        }

        #endregion // AddOrUpdate

        #region GetCached

        /// <summary>
        /// Gets the cached.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public (string json, bool hasValue) GetCached(string key)
        {
            bool hasValue = _mergedConfiguration.TryGetValue(key, out string? json);
            return (json ?? string.Empty, hasValue);
        }

        #endregion // GetCached

        #region GetRaw

        /// <summary>
        /// Gets the raw.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        public (byte[] rawValue, bool hasValue) GetRaw(string key)
        {
            bool hasValue = _rawConfiguration.TryGetValue(key, out byte[]? rawValue);
            return (rawValue ?? Array.Empty<byte>(), hasValue);
        }

        #endregion // GetRaw

        void IDictionary<string, string>.Add(string key, string value)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, string>.ContainsKey(string key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, string>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        bool IDictionary<string, string>.TryGetValue(string key, out string value)
        {
            value = string.Empty;
            return false;
        }

        void ICollection<KeyValuePair<string, string>>.Add(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, string>>.Clear()
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Contains(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        void ICollection<KeyValuePair<string, string>>.CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<KeyValuePair<string, string>>.Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return _mergedConfiguration.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        ICollection<string> IDictionary<string, string>.Keys => throw new NotImplementedException();

        ICollection<string> IDictionary<string, string>.Values => throw new NotImplementedException();

        int ICollection<KeyValuePair<string, string>>.Count => throw new NotImplementedException();

        bool ICollection<KeyValuePair<string, string>>.IsReadOnly => throw new NotImplementedException();

        string IDictionary<string, string>.this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}