﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutationEnv:
        IPermutationBasic<IPermutationEnv>
    {
        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationEnvApp ByAppName();
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
        IPermutationEnvNS ByNamespace(bool strict);
        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvComp ByComponent();
    }
}
