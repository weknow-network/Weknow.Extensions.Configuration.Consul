using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Convention builder stage.
    /// </summary>
    public interface IPermutations :
        IConsulHierarchyBuilder,
        IPermutationApp,
        IPermutationAppComp,
        IPermutationAppNS,
        IPermutationAppNSComp,
        IPermutationComp,
        IPermutationEnv,
        IPermutationEnvApp,
        IPermutationEnvAppComp,
        IPermutationEnvAppNS,
        IPermutationEnvComp,
        IPermutationEnvNS,
        IPermutationEnvNSComp,
        IPermutationNS,
        IPermutationNSComp,
        IPermutationBasic
    {
    }
}
