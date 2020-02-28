using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    public interface IPermutationCommon
    {
        /// <summary>
        /// Builds the hierarchy.
        /// </summary>
        /// <returns></returns>
        IConsulHierarchy Build();
    }
}
