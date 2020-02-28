using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;

namespace Weknow.Extensions.Configuration.Consul
{

    public interface IConsulHierarchy
    {
        /// <summary>
        /// Get the root path (within Consul, Consul root should use '/' separator).
        /// Pattern (Root can be used for): 
        /// * Tenant isolation (multi-tenant).
        /// * Environments (Prod, Dev, Staging)
        /// * Separate department within a company.
        /// </summary>
        string Root { get; }

        /// <summary>
        /// Strict namespace.
        /// </summary>
        bool StrictNamespace { get; }

        /// <summary>
        /// Convention Path.
        /// </summary>
        IImmutableQueue<string> Path { get; } 

    }
}
