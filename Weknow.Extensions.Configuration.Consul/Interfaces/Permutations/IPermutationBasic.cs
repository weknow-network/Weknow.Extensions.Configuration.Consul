using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul.Permutations
{

    /// <summary>
    /// Common convention options.
    /// </summary>
    public interface IPermutationBasic: IPermutationCommon
    {
        /// <summary>
        /// Set next hierarchy to evaluate against 
        /// environment variable key (this is a flexible 
        /// hierarchy which assume that the key will exists).
        /// Pattern (which can be used):
        /// * A/B testing.
        /// * Grouping
        /// </summary>
        /// <param name="environmentVariable">The environment variable.</param>
        /// <returns></returns>
        IPermutationBasic ByCustomEnvironmentVariable(string environmentVariable);
    }

    /// <summary>
    /// Common convention options.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IPermutationBasic<T> : IPermutationCommon
    {
        /// <summary>
        /// Set next hierarchy to evaluate against 
        /// environment variable key (this is a flexible 
        /// hierarchy which assume that the key will exists).
        /// Pattern (which can be used):
        /// * A/B testing.
        /// * Grouping
        /// </summary>
        /// <param name="environmentVariable">The environment variable.</param>
        /// <returns></returns>
        T ByCustomEnvironmentVariable(string environmentVariable);
    }
}
