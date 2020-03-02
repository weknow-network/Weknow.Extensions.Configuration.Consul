using System;
using Consul;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Extensions.Primitives;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Represent a smart proxy for the Consul
    /// </summary>
    internal interface IConsulProxy: IAsyncDisposable
    {
        /// <summary>
        /// Creates Consul client.
        /// </summary>
        /// <param name="path">The path (hierarchic key separate by '/').</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        Task<KVPair[]> GetDataAsync(
            string path,
            CancellationToken cancellationToken);
    }
}