using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using Weknow.Extensions.Configuration.Consul.Permutations;
using static Weknow.Extensions.Configuration.Consul.HierarchicConsts;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Configuration convention builder, 
    /// used for setting the configuration hierarchy convention.
    /// </summary>
    internal class HierarchyBuilder :
        IConsulHierarchy,
        IPermutations
    {
        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyBuilder" /> class.
        /// </summary>
        /// <param name="hostEnvironment">The host environment.</param>
        public HierarchyBuilder(
            IHostEnvironment hostEnvironment)
        {
            _deploymentEnvironment = hostEnvironment.EnvironmentName;
            _applicationName = hostEnvironment.ApplicationName;
            _hostEnvironment = hostEnvironment;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HierarchyBuilder"/> class.
        /// </summary>
        /// <param name="copyFrom">The copy from.</param>
        private HierarchyBuilder(
            HierarchyBuilder copyFrom)
        {
            _root = copyFrom._root;
            _strictNS = copyFrom._strictNS;
            _path = copyFrom._path;
            _applicationName = copyFrom._applicationName;
            _deploymentEnvironment = copyFrom._deploymentEnvironment;
            _hostEnvironment = copyFrom._hostEnvironment;
        }

        #endregion // Ctor

        #region DeploymentEnvironment

        private readonly string _deploymentEnvironment;
        /// <summary>
        /// Gets the deployment environment.
        /// </summary>
        string IConsulHierarchy.DeploymentEnvironment => _deploymentEnvironment;

        #endregion // DeploymentEnvironment

        #region ApplicationName

        private readonly string _applicationName;
        private readonly IHostEnvironment _hostEnvironment;

        /// <summary>
        /// Gets the name of the application.
        /// </summary>
        string IConsulHierarchy.ApplicationName => _applicationName;

        #endregion // ApplicationName

        #region StrictNamespace

        private bool _strictNS;
        /// <summary>
        /// Strict namespace.
        /// </summary>
        bool IConsulHierarchy.StrictNamespace => _strictNS;

        #endregion // StrictNamespace

        #region Root

        private string _root = string.Empty;
        /// <summary>
        /// Get the root path (within Consul, Consul root should use '/' separator).
        /// Pattern (Root can be used for): 
        /// * Tenant isolation (multi-tenant).
        /// * Environments (Prod, Dev, Staging)
        /// * Separate department within a company.
        /// </summary>
        string IConsulHierarchy.Root => _root;

        #endregion // Root

        #region Path

        private IImmutableQueue<string> _path = ImmutableQueue<string>.Empty;

        /// <summary>
        /// Convention Path.
        /// </summary>
        IImmutableQueue<string> IConsulHierarchy.Path => _path;

        #endregion // Path

        #region Build

        /// <summary>
        /// Builds the convention.
        /// </summary>
        /// <returns></returns>
        IConsulHierarchy IPermutationCommon.Build()
        {
            return this;
        }

        #endregion // Build

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
        IPermutationRoot IConsulHierarchyBuilder.EntryPath(string rootPath)
        {
            var next = new HierarchyBuilder(this);
            next._root = rootPath;
            return next;
        }

        /// <summary>
        /// Set the root path (within Consul, Consul root should use '/' separator).
        /// Pattern (Root can be used for):
        /// * Tenant isolation (multi-tenant).
        /// * Environments (Prod, Dev, Staging)
        /// * Separate department within a company.
        /// </summary>
        /// <param name="rootPathFactory">The root path where the configuration start.
        /// for example you can use root per tenant.</param>
        /// <returns></returns>
        IPermutationRoot IConsulHierarchyBuilder.EntryPath(
            Func<IHostEnvironment, string> rootPathFactory)
        {
            string path = rootPathFactory(_hostEnvironment);
            IConsulHierarchyBuilder self = this;
            return self.EntryPath(path);
        }

        #endregion // SetRootPath

        #region ByAppName

        /// <summary>
        /// Add to path.
        /// </summary>
        /// <returns></returns>
        private HierarchyBuilder ByAppName()
        {
            var next = new HierarchyBuilder(this);
            next._path = _path.Enqueue(APP_KEY);
            return next;
        }

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        IPermutationApp IPermutationRoot.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationAppComp IPermutationComp.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationEnvApp IPermutationEnv.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppComp IPermutationEnvComp.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppNS IPermutationEnvNS.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationBasic IPermutationEnvNSComp.ByAppName() => ByAppName();

        /// <summary>
        /// Set next hierarchy to math the application / micro-service name.
        /// </summary>
        /// <returns></returns>
        IPermutationAppNSComp IPermutationNSComp.ByAppName() => ByAppName();

        #endregion // ByAppName

        #region ByComponent

        /// <summary>
        /// Add to path.
        /// </summary>
        /// <returns></returns>
        private HierarchyBuilder ByComponent()
        {
            var next = new HierarchyBuilder(this);
            next._path = _path.Enqueue(COMP_KEY);
            return next;
        }

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationComp IPermutationRoot.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationAppComp IPermutationApp.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationAppNSComp IPermutationAppNS.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvComp IPermutationEnv.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationBasic IPermutationEnvAppNS.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppComp IPermutationEnvApp.ByComponent() => ByComponent();

        /// <summary>
        /// Set next hierarchy to math the consumer component name (without namespace).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvNSComp IPermutationEnvNS.ByComponent() => ByComponent();

        #endregion // ByComponent

        #region ByEnvironment

        /// <summary>
        /// Add to path.
        /// </summary>
        /// <returns></returns>
        private HierarchyBuilder ByEnvironment()
        {
            var next = new HierarchyBuilder(this);
            next._path = _path.Enqueue(ENV_KEY);
            return next;
        }

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        IPermutationEnv IPermutationRoot.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvApp IPermutationApp.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppComp IPermutationAppComp.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvAppNS IPermutationAppNS.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationBasic IPermutationAppNSComp.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvComp IPermutationComp.ByEnvironment() => ByEnvironment();

        /// <summary>
        /// Set next hierarchy to math the deployment environment (Development, Production, Staging, etc...).
        /// </summary>
        /// <returns></returns>
        IPermutationEnvNSComp IPermutationNSComp.ByEnvironment() => ByEnvironment();

        #endregion // ByEnvironment

        #region ByNamespace

        /// <summary>
        /// Add to path.
        /// </summary>
        /// <returns></returns>
        private HierarchyBuilder ByNamespace(bool strict)
        {
            var next = new HierarchyBuilder(this);
            next._path = _path.Enqueue(NS_KEY);
            next._strictNS = strict;
            return next;
        }

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationNS IPermutationRoot.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationAppNS IPermutationApp.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationAppNSComp IPermutationAppComp.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationNSComp IPermutationComp.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationEnvNS IPermutationEnv.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationBasic IPermutationEnvAppComp.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationEnvAppNS IPermutationEnvApp.ByNamespace(bool strict) => ByNamespace(strict);

        /// <summary>
        /// Set next hierarchy to math the consumer component's namespace.
        /// </summary>
        /// <param name="strict">if set to <c>true</c> evaluate against the full namespace.
        /// otherwise can evaluate against start-with part of the namespace
        /// (breakdown by the namespace '.', don't support partial phrases
        /// except the '.' separated phrases)</param>
        /// <returns></returns>
        IPermutationEnvNSComp IPermutationEnvComp.ByNamespace(bool strict) => ByNamespace(strict);

        #endregion // ByNamespace

        #region ByCustomEnvironmentVariable

        /// <summary>
        /// Add to path.
        /// </summary>
        /// <returns></returns>
        private HierarchyBuilder ByCustomEnvironmentVariable(string environmentVariable)
        {
            var next = new HierarchyBuilder(this);
            next._path = _path.Enqueue(environmentVariable);
            return next;
        }

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
        IPermutationRoot IPermutationBasic<IPermutationRoot>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationApp IPermutationBasic<IPermutationApp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationAppComp IPermutationBasic<IPermutationAppComp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationAppNS IPermutationBasic<IPermutationAppNS>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationAppNSComp IPermutationBasic<IPermutationAppNSComp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnv IPermutationBasic<IPermutationEnv>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnvApp IPermutationBasic<IPermutationEnvApp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnvComp IPermutationBasic<IPermutationEnvComp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnvAppNS IPermutationBasic<IPermutationEnvAppNS>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationBasic IPermutationBasic.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnvNS IPermutationBasic<IPermutationEnvNS>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationEnvNSComp IPermutationBasic<IPermutationEnvNSComp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

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
        IPermutationNSComp IPermutationBasic<IPermutationNSComp>.ByCustomEnvironmentVariable(string environmentVariable) => ByCustomEnvironmentVariable(environmentVariable);

        #endregion // ByCustomEnvironmentVariable
    }
}
