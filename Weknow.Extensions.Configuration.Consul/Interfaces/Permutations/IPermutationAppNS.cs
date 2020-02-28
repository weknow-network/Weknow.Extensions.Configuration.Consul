using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationAppNS :
        IPermutationBasic<IPermutationAppNS>
    {
        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppNS ByEnvironment();
        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationAppNSComp ByComponent();
    }
}
