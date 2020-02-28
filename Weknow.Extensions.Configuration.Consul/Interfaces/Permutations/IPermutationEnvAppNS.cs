using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationEnvAppNS :
        IPermutationBasic<IPermutationEnvAppNS>
    {
        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationBasic ByComponent();
    }
}
