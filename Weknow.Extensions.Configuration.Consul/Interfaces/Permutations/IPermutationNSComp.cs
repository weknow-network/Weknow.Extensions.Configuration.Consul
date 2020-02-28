using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationNSComp :
        IPermutationBasic<IPermutationNSComp>
    {
        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvNSComp ByEnvironment();
        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationAppNSComp ByAppName();
    }
}
