using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationApp :
        IPermutationBasic<IPermutationApp>
    {
        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvApp ByEnvironment();
        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">
        /// if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace 
        /// (breakdown by the namespace '.', don't support partial phrases 
        /// except the '.' separated phrases) 
        /// </param>
        /// <returns></returns>
        IPermutationAppNS ByNamespace(bool strict);
        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationAppComp ByComponent();
    }
}
