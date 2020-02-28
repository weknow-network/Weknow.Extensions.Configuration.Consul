using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationEnvNS :
        IPermutationBasic<IPermutationEnvNS>
    {
        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppNS ByAppName();
        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvNSComp ByComponent();
    }
}
