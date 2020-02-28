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
    public class ConsulClientOptions : IConsulClientOptions
    {
        private const string CONSUL_URL_KEY = "consul-url";

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulClientOptions"/> class.
        /// </summary>
        /// <param name="tenant">The tenant represent the base-path of the configuration.
        /// The most general configuration consider as a tenant.
        /// Each service can serve a single tenant (the configuration consume
        /// via dependency-injection, therefore don't resolved at runtime).</param>
        /// <param name="address">Optional address of the console service.
        /// If missing will check:
        /// * Environment variable: consul-url
        /// * default (if don't find anything): http://127.0.0.1:8500
        /// .</param>
        /// <param name="configOverride">Enable to override configuration.</param>
        /// <param name="httpClientOverride">Enable to override HTTP client.</param>
        /// <param name="httpClientHandlerOverride">Enable to override HTTP client handler.</param>
        public ConsulClientOptions(
            string? address = null,
            Action<ConsulClientConfiguration>? configOverride = null,
            Action<HttpClient>? httpClientOverride = null,
            Action<HttpClientHandler>? httpClientHandlerOverride = null)
        {
            Address = address ??
                        Environment.GetEnvironmentVariable(CONSUL_URL_KEY) ??
                        "http://127.0.0.1:8500";
            ConfigOverride = configOverride;
            HttpClientOverride = httpClientOverride;
            HttpClientHandlerOverride = httpClientHandlerOverride;
        }

        #endregion // Ctor

        #region Address

        /// <summary>
        /// Consul address
        /// </summary>
        public string Address { get; }

        #endregion // Address

        #region ConfigOverride

        /// <summary>
        /// Enable to override configuration.
        /// </summary>
        public Action<ConsulClientConfiguration>? ConfigOverride { get; set; }

        #endregion // ConfigOverride

        #region HttpClientOverride

        /// <summary>
        /// Enable to override HTTP client
        /// </summary>
        public Action<HttpClient>? HttpClientOverride { get; set; }

        #endregion // HttpClientOverride

        #region HttpClientHandlerOverride

        /// <summary>
        /// Enable to override HTTP client handler.
        /// </summary>
        public Action<HttpClientHandler>? HttpClientHandlerOverride { get; set; }

        #endregion // HttpClientHandlerOverride
    }
}
