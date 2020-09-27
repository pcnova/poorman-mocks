// -----------------------------------------------------------------------------
// Copyright (c) 2020 Paul C.
// Licensed under the MPL 2.0 license. See LICENSE.txt for full information.
// -----------------------------------------------------------------------------

namespace PoorMan.Mocks.Extensions
{
    using System.Reflection;
    using System.Runtime.CompilerServices;

    /// <summary>
    ///     Contains methods that extend the <see cref="MemberInfo"/> type.
    /// </summary>
    internal static class MemberInfoExtensions
    {
        /// <summary>
        ///     Gets a value indicating whether the member is compiler-generated.
        /// </summary>
        /// <param name="member">
        ///     The member to evaluate.
        /// </param>
        /// <returns>
        ///     <c>True</c> if the member is marked as compiler-generated;
        ///     <c>false</c> otherwise.
        /// </returns>
        public static bool IsGenerated(this MemberInfo member)
        {
            return member.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }
    }
}