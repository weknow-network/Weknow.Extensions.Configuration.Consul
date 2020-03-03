// Copyright (c) Winton. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using System.Collections.Generic;
using Consul;

namespace Weknow.Extensions.Configuration.Consul
{
    /// <summary>
    /// KVPair Comparer
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IComparer{Consul.KVPair}" />
    internal class KVPairComparer : IComparer<KVPair>
    {
        /// <summary>
        /// Singleton
        /// </summary>
        public static readonly IComparer<KVPair> Default = new KVPairComparer();
        
        #region Compare

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.
        /// Value
        /// Meaning
        /// Less than zero
        /// <paramref name="x" /> is less than <paramref name="y" />.
        /// Zero
        /// <paramref name="x" /> equals <paramref name="y" />.
        /// Greater than zero
        /// <paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public int Compare(KVPair? x, KVPair? y)
        {
            string xkey = x?.Key ?? string.Empty;
            string ykey = y?.Key ?? string.Empty;
            string keyX = Restruct(xkey);
            string keyY = Restruct(ykey);
            return string.Compare(keyX, keyY);
        }

        #endregion // Compare

        #region Restruct

        /// <summary>
        /// Re-structure the specified key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns></returns>
        private string Restruct(string key)
        {
            int len = key.LastIndexOf('/');
            if (len == -1)
                return key;
            string part1 = key.Substring(len + 1);
            string part2 = key.Substring(0, len);
            return $"{part1}/{part2}";
        }

        #endregion // Restruct
    }
}