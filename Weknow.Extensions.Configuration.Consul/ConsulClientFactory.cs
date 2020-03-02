using Consul;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Consul client factory
    /// </summary>
    /// <seealso cref="Weknow.Extensions.Configuration.Consul.IConsulFactory" />
    internal sealed class ConsulClientFactory: IConsulFactory
    {
        private readonly IConsulClientOptions _options;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulClientFactory" /> class.
        /// </summary>
        /// <param name="consulClientCreationOptions">The consul client creation options.</param>
        public ConsulClientFactory(
            IConsulClientOptions consulClientCreationOptions)
        {
            _options = consulClientCreationOptions;
        }

        #endregion // Ctor

        #region Create

        /// <summary>
        /// Creates Consul client.
        /// </summary>
        /// <returns></returns>
        IConsulClient IConsulFactory.Create()
        {
            var client = new ConsulClient(
                (ConsulClientConfiguration consulConfig) =>
                {
                    consulConfig.Address = new Uri(_options.Address);
                    _options.ConfigOverride?.Invoke(consulConfig);
                },
                _options.HttpClientOverride ?? ((c) => { }),
                (HttpClientHandler clientHandler) =>
                {
                    //disable proxy of HttpcLientHandler  
                    clientHandler.Proxy = null;
                    clientHandler.UseProxy = false;
                    _options.HttpClientHandlerOverride?.Invoke(clientHandler);
                });
            return client;
        }

        #endregion // Create
    }
}
