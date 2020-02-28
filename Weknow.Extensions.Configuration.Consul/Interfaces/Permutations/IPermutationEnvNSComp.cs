using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationEnvNSComp :
        IPermutationBasic<IPermutationEnvNSComp>
    {
        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationBasic ByAppName();
    }
}
