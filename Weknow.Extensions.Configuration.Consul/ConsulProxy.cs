using Consul;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Consul executer abstraction
    /// </summary>
    /// <seealso cref="Weknow.Extensions.Configuration.Consul.IConsulProxy" />
    internal sealed class ConsulProxy : IConsulProxy
    {
        private static readonly TimeSpan TIMEOUT = TimeSpan.FromSeconds(30);
        private ulong _lastIndex = 0;
        private readonly IConsulFactory _factory; // can use to renew the client on fatal state
        private IConsulClient _client;

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="ConsulProxy"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public ConsulProxy(
            IConsulFactory factory)
        {
            _client = factory.Create();
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
            _client.Dispose();
            return new ValueTask();
        }

        #endregion // DisposeAsync

        #region GetDataAsync

        /// <summary>
        /// Gets the data.
        /// </summary>
        /// <param name="path">The path (hierarchic key separate by '/').</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        public async Task<KVPair[]> GetDataAsync(
            string path,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var queryOptions = new QueryOptions
            {
                WaitTime = TIMEOUT,
                WaitIndex = _lastIndex
            };

            QueryResult<KVPair[]> consulResponse =
                await _client
                    .KV
                    .List(path, queryOptions, cancellationToken);


            HttpStatusCode status = consulResponse.StatusCode;
            if (status != HttpStatusCode.NotFound &&
                status != HttpStatusCode.OK)
            {
                throw new Exception($"Error loading configuration from consul. Status code: {status}.");
            }

            LastContact = consulResponse.LastContact;
            _lastIndex = consulResponse.LastIndex;

            return consulResponse.StatusCode switch
            {
                HttpStatusCode.OK => consulResponse.Response,
                _ => Array.Empty<KVPair>()
            };
        }

        #endregion // GetDataAsync

        #region LastContact

        /// <summary>
        /// Gets the last contact.
        /// </summary>
        public TimeSpan LastContact { get; private set; } = Timeout.InfiniteTimeSpan;

        #endregion // LastContact
    }
}
