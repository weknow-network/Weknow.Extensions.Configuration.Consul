using Consul;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Consul client creation options
    /// </summary>
    public interface IConsulClientOptions
    {
        /// <summary>
        /// Gets the consul address.
        /// </summary>
        string Address { get; }
        /// <summary>
        /// Option to override configuration aspects.
        /// </summary>
        Action<ConsulClientConfiguration>? ConfigOverride { get; }
        /// <summary>
        /// Option to override HTTP Client aspects.
        /// </summary>
        Action<HttpClient>? HttpClientOverride { get; }
        /// <summary>
        /// Option to override HTTP Client Handler aspects.
        /// </summary>
        Action<HttpClientHandler>? HttpClientHandlerOverride { get; }
    }
}
