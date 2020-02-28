using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Weknow.Extensions.Configuration.Consul.Permutations;

namespace Weknow.Extensions.Configuration.Consul
{

    /// <summary>
    /// Convention builder stage (after setting the root).
    /// </summary>
    public interface IConsulHierarchyBuilder :
                        IPermutationRoot
    {
        #region SetRootPath

        /// <summary>
        /// Set the root path (within Consul, Consul root should use '/' separator).
        /// Pattern (Root can be used for): 
        /// * Tenant isolation (multi-tenant).
        /// * Environments (Prod, Dev, Staging)
        /// * Separate department within a company.
        /// </summary>
        /// <param name="rootPath">
        /// The root path where the configuration start.
        /// for example you can use root per tenant.
        /// </param>
        /// <returns></returns>
        IPermutationRoot SetRootPath(string rootPath);

        #endregion // SetRootPath
    }
}
