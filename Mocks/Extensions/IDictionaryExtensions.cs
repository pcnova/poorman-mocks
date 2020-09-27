// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Extensions
{
    using System.Collections.Generic;

    /// <summary>
    ///     Contains methods that extend the <see cref="IDictionary{TKey, TValue}"/>
    ///     type.
    /// </summary>
    internal static class IDictionaryExtensions
    {
        /// <summary>
        ///     Adds a value with the specified key to the dictionary, or updates
        ///     the value if an entry for that key already exists.
        /// </summary>
        /// <typeparam name="TKey">
        ///     The type of key to add or update.
        /// </typeparam>
        /// <typeparam name="TValue">
        ///     The type of value to add or update.
        /// </typeparam>
        /// <param name="dictionary">
        ///     The dictionary to update.
        /// </param>
        /// <param name="key">
        ///     The key of the entry to add or update.
        /// </param>
        /// <param name="value">
        ///     The value to add or update.
        /// </param>
        public static void AddOrUpdate<TKey, TValue>(
            this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            if (!dictionary.ContainsKey(key))
            {
                dictionary.Add(key, value);
            }
            else
            {
                dictionary[key] = value;
            }
        }
    }
}
