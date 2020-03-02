using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Data;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// Hierarchic constants
    /// </summary>
    internal static class HierarchicConsts
    {
        /// <summary>
        /// The env key
        /// </summary>
        public const string ENV_KEY = "~ENV~";
        /// <summary>
        /// The application key
        /// </summary>
        public const string APP_KEY = "~APP~";
        /// <summary>
        /// The ns key
        /// </summary>
        public const string NS_KEY = "~NS~";
        /// <summary>
        /// The comp key
        /// </summary>
        public const string COMP_KEY = "~COMP~";

    }
}
