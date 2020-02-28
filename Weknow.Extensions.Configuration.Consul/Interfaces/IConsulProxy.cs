using Consul;
using System.Threading.Tasks;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Represent a smart proxy for the Consul
    /// </summary>
    internal interface IConsulProxy
    {
        /// <summary>
        /// Creates Consul client.
        /// </summary>
        /// <returns></returns>
        IConsulClient Create();
    }
}